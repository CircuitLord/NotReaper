using NotReaper.IO;
using NotReaper.Managers;
using NotReaper.Models;
using NotReaper.Targets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotReaper.Tools.ErrorChecker
{

    public class ErrorChecker : MonoBehaviour
    {

        //Hidden public values
        //[HideInInspector] public static AudicaFile audicaFile;

        [SerializeField] private DifficultyManager difficultyManager;

        private float chainLeadTime = 360;
        private float sustainLeadTime = 360;
        private const float TickBeatConst = 480;

        public void RunErrorCheck()
        {
            /* retrieve orderedNotes
             * parse notes for errors
             * export error messages to txt
             */

            //retrieve orderedNotes and difficulty label
            List<Target> notes = Timeline.orderedNotes;
            int difficulty = difficultyManager.loadedIndex; //0 Expert, 1 Advanced, 2 Standard, 3 Beginner
            string difficultyLabel;
            switch(difficulty)
            {
                case 0:
                    difficultyLabel = "EXPERT";
                    break;
                case 1:
                    difficultyLabel = "ADVANCED";
                    break;
                case 2:
                    difficultyLabel = "STANDARD";
                    break;
                case 3:
                    difficultyLabel = "BEGINNER";
                    break;
                default:
                    difficultyLabel = "EXPERT";
                    break;

            }

            //parse and generate error log
            string outputMessage = "";
            outputMessage = ParseCues(notes, outputMessage, difficulty, difficultyLabel);

            //export
            //output to txt
            System.IO.File.WriteAllText(@"D:\test\test.txt", outputMessage);
        }

        private string ParseCues(List<Target> targetCues, string output, int difficulty, string label)
        {
            //error log
            List<ErrorLogEntry> errorLog = new List<ErrorLogEntry>();

            //references to previous targets to help with parsing
            TargetData prevTarget = new TargetData();       //dual purpose reference. This is the previous target regardless if it's RH or LH; also used in the RH/LH backtrack checks so I don't have to copy paste code.
            TargetData prevRHTarget = new TargetData();
            TargetData prevLHTarget = new TargetData();
            TargetData prevMeleeTarget = new TargetData();
            TargetData lastLastTarget = new TargetData();
            int consecutiveCounter = 0;

            ////////////////////////
            // main parsing block //
            ////////////////////////

            foreach (Target curTarget in targetCues)
            {
                ///////////////////////
                // standalone checks //
                ///////////////////////
                

                //cues without hitsounds
                if (!HasHitSound(curTarget))
                {
                    errorLog.Add(new ErrorLogEntry(curTarget.data.beatTime, "ERROR, target has an invalid hitsound."));
                }

                //////////////////////////////
                // general backtrack checks //
                //////////////////////////////

                // consecutive rhythm check
                // if lower difficulties and not chain node
                if (difficulty != 0 && (!prevTarget.behavior.Equals(TargetBehavior.Chain) && !curTarget.data.behavior.Equals(TargetBehavior.Chain)))
                {
                    float rhythmLimit = SetRhythmLimit(difficulty);
                    int countLimit = SetCountLimit(difficulty);
                    float beatTimeDiff = curTarget.data.beatTime - prevTarget.beatTime;
                    if (beatTimeDiff > rhythmLimit)
                    {
                        //reset counter
                        consecutiveCounter = 0;
                    }
                    else if(beatTimeDiff != 0 && beatTimeDiff < rhythmLimit)
                    {
                        //straight up too fast
                        errorLog.Add(new ErrorLogEntry(curTarget.data.beatTime, "WARNING, for " + label + ", this target happens too soon after the previous target."));
                    }
                    else if(curTarget.data.beatTime - prevTarget.beatTime == rhythmLimit)
                    {
                        // consecutive 8th notes on one hand
                        if(difficulty==2 && prevTarget.handType.Equals(curTarget.data.handType))
                        {
                            Debug.Log("consecutive 8t notes on one hand");
                            errorLog.Add(new ErrorLogEntry(curTarget.data.beatTime, "WARNING, in " + label + ", consecutive 8th notes on one hand are not recommended."));
                        }
                        
                        //increment counter, if it gets above the countLimit, log an error
                        consecutiveCounter++;

                        if(consecutiveCounter >= countLimit)
                        {
                            //TODO convert rhythmLimit to quarter note, eigth note, etc.
                            errorLog.Add(new ErrorLogEntry(curTarget.data.beatTime, "WARNING, in " + label + ", having more than " + countLimit + " consecutive " + rhythmLimit + " targets is not recommended."));
                        }
                    }

                }

                //simultaneous target checks
                //chains and melees don't count
                if (prevTarget.beatTime == curTarget.data.beatTime && (!prevTarget.behavior.Equals(TargetBehavior.Chain) && !curTarget.data.behavior.Equals(TargetBehavior.Chain)) && (!prevTarget.behavior.Equals(TargetBehavior.Melee) && !curTarget.data.behavior.Equals(TargetBehavior.Melee)))
                {
                    // same pitch
                    if (prevTarget.position == curTarget.data.position)
                    {
                        errorLog.Add(new ErrorLogEntry(prevTarget.beatTime, "ERROR, there are multiple targets occupying the same position."));
                    }

                    // same color
                    if (prevTarget.handType.Equals(curTarget.data.handType) && !prevTarget.handType.Equals(TargetHandType.Either))
                    {
                        errorLog.Add(new ErrorLogEntry(prevTarget.beatTime, "ERROR, the " + prevTarget.handType + " hand has multiple targets at the same time."));
                    }

                    // ADVANCED and lower
                    if (difficulty > 0)
                    {
                        
                        //simultaneous shot and melee
                        if (IsSimultaneousShotAndMelee(prevTarget,curTarget))
                        {
                            errorLog.Add(new ErrorLogEntry(prevTarget.beatTime, "WARNING, in " + label + ", simultaneous melee and shot targets are not recommended."));
                        }
                        //simultaneous targets must be within 4 spaces apart for Advanced, 3 for Standard/Beginner
                        else
                        {
                            float distance = (difficulty == 1 ? 4 : 3);
                            if (!IsCloseEnough(prevTarget, curTarget, distance))
                            {
                                errorLog.Add(new ErrorLogEntry(prevTarget.beatTime, "WARNING, in " + label + ", simultaneous targets more than " + distance + " spaces apart are not recommended."));
                            }
                        }
                    }

                }

                ////////////////////////////////////
                // lower difficulty lead-in times //
                ////////////////////////////////////

                // slotted notes
                if (IsSlottedNote(curTarget.data) && difficulty != 0)
                {
                    //ADVANCED
                    if (difficulty == 1)
                    {
                        if (!IsSlottedNote(prevTarget) && InsufficientBreakAfterPreviousTarget(prevTarget, curTarget, 2*TickBeatConst))
                        {
                            errorLog.Add(new ErrorLogEntry(prevTarget.beatTime, "WARNING, in ADVANCED, it is recommended to have at least 2 beats of lead-in time before introducing a horizontal/vertical slotted note."));
                        }
                    }
                    else // no slotted notes for STANDARD or BEGINNER
                    {
                        errorLog.Add(new ErrorLogEntry(prevTarget.beatTime, "WARNING, in " + label + ", use of horizontal/vertical slotted notes is not recommended."));
                    }
                }

                //////////////////
                // melee checks //
                //////////////////

                //prev low solo melee check
                if (prevTarget.behavior.Equals(TargetBehavior.Melee) && IsLowMelee(prevTarget))
                {
                    if (IsLowSoloMelee(lastLastTarget, prevTarget, curTarget.data))
                    {
                        errorLog.Add(new ErrorLogEntry(prevTarget.beatTime, "WARNING, this Melee Target is by itself in the lower slot. Single melees should be in the higher slot. Only use the lower slot for making simultaneous melees stacked on top of each other."));
                    }
                }

                if (curTarget.data.behavior.Equals(TargetBehavior.Melee)) {
                    //non melee hitsound
                    if (!IsMeleeHitSound(curTarget))
                    {
                        errorLog.Add(new ErrorLogEntry(curTarget.data.beatTime, "WARNING, Melee Target doesn't have a Melee hitsound."));
                    }

                    //low melee
                    if (IsLowMelee(curTarget.data))
                    {
                        // won't log an ERROR, yet.
                        // will check on the next cycle of the parser if we have a low solo melee.
                        // for now, save an extra reference to prevTarget.
                        lastLastTarget = prevTarget;
                    }

                    //update previous melee reference
                    prevMeleeTarget = curTarget.data;
                }

                ////////////////////////////
                // LH/RH backtrack checks //
                ////////////////////////////

                if (curTarget.data.handType.Equals(TargetHandType.Right)){ prevTarget = prevRHTarget; }
                else { prevTarget = prevLHTarget; }

                if (!curTarget.data.behavior.Equals(TargetBehavior.Melee))
                {

                    //short break after sustain 
                    if (prevTarget.behavior.Equals(TargetBehavior.Hold))
                    {
                        if (InsufficientBreakAfterSustain(prevTarget,curTarget,sustainLeadTime))
                        {
                            errorLog.Add(new ErrorLogEntry(prevTarget.beatTime, "WARNING, the time between the end of this sustain target and the next target on the same hand is very short; at least " + sustainLeadTime + " is recommended."));
                        }
                    }
                   
                    //short break after chain node
                    if (prevTarget.behavior.Equals(TargetBehavior.Chain) && !curTarget.data.behavior.Equals(TargetBehavior.Chain))
                    {
                        if (InsufficientBreakAfterPreviousTarget(prevTarget,curTarget,chainLeadTime))
                        {
                            errorLog.Add(new ErrorLogEntry(prevTarget.beatTime, "WARNING, the time between the end of this chain and the next target on the same hand is very short; at least " + chainLeadTime + " is recommended."));
                        }
                    }

                    //headless chains
                    if (curTarget.data.behavior.Equals(TargetBehavior.Chain) && !(prevTarget.behavior.Equals(TargetBehavior.Chain) || prevTarget.behavior.Equals(TargetBehavior.ChainStart)))
                    {
                        errorLog.Add(new ErrorLogEntry(curTarget.data.beatTime, "ERROR, this chain node does not have a proper Chain Start."));
                    }

                    // update prev target
                    if (curTarget.data.handType.Equals(TargetHandType.Right)) { prevRHTarget = curTarget.data; }
                    else { prevLHTarget = curTarget.data; }

                }

                //Update previous target reference
                prevTarget = curTarget.data;
            }


            // string output
            output = "Error checker has detected " + errorLog.Count + " errors/warnings." + System.Environment.NewLine;
            foreach(ErrorLogEntry item in errorLog)
            {
                output = output + "[" + (item.beatTime*TickBeatConst) + "] " + item.errorDesc + System.Environment.NewLine;
            }
            return output;
        }



        // checker functions
        // TODO refactor smaller functions back into parser code block

        private bool HasHitSound(Target targetCue)
        {
            return Enum.IsDefined(typeof(TargetVelocity), targetCue.data.velocity);
        }

        private bool IsMeleeHitSound(Target targetCue)
        {
            return targetCue.data.velocity.Equals(TargetVelocity.Melee);
        }

        private bool IsLowMelee(TargetData targetCue)
        {
            return targetCue.position.y < 0;
        }

        private bool IsLowSoloMelee(TargetData prev, TargetData lm, TargetData next)
        {
            if (prev.behavior.Equals(TargetBehavior.Melee) && prev.beatTime == lm.beatTime && prev.position.x * lm.position.x > 0)
            { return false; }
            if (next.behavior.Equals(TargetBehavior.Melee) && next.beatTime == lm.beatTime && next.position.x * lm.position.x > 0)
            { return false; }
                return true;
        }

        private bool InsufficientBreakAfterSustain(TargetData prevTarget, Target curTarget, float leadTime)
        {
            return curTarget.data.beatTime * TickBeatConst - (prevTarget.beatTime * TickBeatConst + prevTarget.beatLength) < leadTime;
        }

        private bool InsufficientBreakAfterPreviousTarget(TargetData prevTarget, Target curTarget, float leadTime)
        {
            return curTarget.data.beatTime * TickBeatConst - prevTarget.beatTime * TickBeatConst < leadTime;
        }

        private bool IsCloseEnough(TargetData prevTarget, Target curTarget, float distance)
        {
            float modDist = distance * 1.1f;
            float x1 = prevTarget.position.x;
            float y1 = prevTarget.position.y;
            float x2 = curTarget.data.position.x;
            float y2 = curTarget.data.position.y;

            float result = (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
            return result <= modDist * modDist;
        }

        private bool IsSimultaneousShotAndMelee(TargetData prevTarget,Target curTarget)
        {
            bool a, b;
            a = prevTarget.behavior.Equals(TargetBehavior.Melee) && !curTarget.data.behavior.Equals(TargetBehavior.Melee);
            b = !prevTarget.behavior.Equals(TargetBehavior.Melee) && curTarget.data.behavior.Equals(TargetBehavior.Melee);
            return (a || b);
        }

        private bool IsSlottedNote(TargetData target)
        {
            return (target.behavior.Equals(TargetBehavior.Horizontal) || target.behavior.Equals(TargetBehavior.Vertical));
        }

        private int SetCountLimit(int difficulty)
        {
            return (difficulty == 2 || difficulty == 3 ? 2 : 3);
        }

        private float SetRhythmLimit(int difficulty)
        {
            float limit;
            switch (difficulty)
            {
                case 1:
                    limit = 0.25f;
                    break;
                case 2:
                    limit = 0.5f;
                    break;
                case 3:
                    limit = 1f;
                    break;
                default:
                    limit = 0.25f;
                    break;
            }
            return limit;
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


    }
}
