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

        public void RunErrorCheck()
        {
            /* retrieve orderedNotes
             * parse notes for errors
             * export error messages to txt
             */

            //retrieve orderedNotes and difficulty label
            List<Target> notes = Timeline.orderedNotes;
            int difficulty = difficultyManager.loadedIndex;

            //parse and generate error log
            string outputMessage = "";
            outputMessage = ParseCues(notes, outputMessage);

            //export
            //output to txt
            System.IO.File.WriteAllText(@"D:\test\test.txt", outputMessage);
        }

        private string ParseCues(List<Target> targetCues, string output)
        {
            //error log
            List<ErrorLogEntry> errorLog = new List<ErrorLogEntry>();

            //references to previous targets to help with parsing
            TargetData prevTarget = new TargetData();       //dual purpose reference. This is the previous target regardless if it's RH or LH; also used in the RH/LH backtrack checks so I don't have to copy paste code.
            TargetData prevRHTarget = new TargetData();
            TargetData prevLHTarget = new TargetData();
            TargetData prevMeleeTarget = new TargetData();
            TargetData lastLastTarget = new TargetData();   

            ////////////////////////
            // main parsing block //
            ////////////////////////

            foreach (Target item in targetCues)
            {
                //////////////////////
                // standalone check //
                //////////////////////
                

                //cues without hitsounds
                if (!HasHitSound(item))
                {
                    errorLog.Add(new ErrorLogEntry(item.data.beatTime, "ERROR Target has an invalid hitsound."));
                }

                //////////////////////////////
                // general backtrack checks //
                //////////////////////////////

                //simultaneous target checks
                if (prevTarget.beatTime == item.data.beatTime)
                {
                    Debug.Log("Simultaneous target detected");
                    // same pitch
                    if (prevTarget.position == item.data.position)
                    {
                        errorLog.Add(new ErrorLogEntry(prevTarget.beatTime, "ERROR There are multiple targets occupying the same position."));
                    }

                    // same color
                    if (prevTarget.handType.Equals(item.data.handType) && !prevTarget.handType.Equals(TargetHandType.Either))
                    {
                        errorLog.Add(new ErrorLogEntry(prevTarget.beatTime, "ERROR The " + prevTarget.handType + " hand has multiple targets at the same time."));
                    }

                }

                //////////////////
                // melee checks //
                //////////////////

                //prev low solo melee check
                if (prevTarget.behavior.Equals(TargetBehavior.Melee) && IsLowMelee(prevTarget))
                {
                    if (IsLowSoloMelee(lastLastTarget, prevTarget, item.data))
                    {
                        errorLog.Add(new ErrorLogEntry(prevTarget.beatTime, "WARNING This Melee Target is by itself in the lower slot. Single melees should be in the higher slot. Only use the lower slot for making simultaneous melees stacked on top of each other."));
                    }
                }

                if (item.data.behavior.Equals(TargetBehavior.Melee)) {
                    //non melee hitsound
                    if (!IsMeleeHitSound(item))
                    {
                        errorLog.Add(new ErrorLogEntry(item.data.beatTime, "WARNING Melee Target doesn't have a Melee hitsound."));
                    }

                    //low melee
                    if (IsLowMelee(item.data))
                    {
                        // won't log an error yet.
                        // will check on the next cycle of the parser if we have a low solo melee.
                        // for now, save an extra reference to prevTarget.
                        lastLastTarget = prevTarget;
                    }

                    //update previous melee reference
                    prevMeleeTarget = item.data;
                }

                ////////////////////////////
                // LH/RH backtrack checks //
                ////////////////////////////

                if (item.data.handType.Equals(TargetHandType.Right)){ prevTarget = prevRHTarget; }
                else { prevTarget = prevLHTarget; }

                if (!item.data.behavior.Equals(TargetBehavior.Melee))
                {

                    //short break after sustain 
                    if (prevTarget.behavior.Equals(TargetBehavior.Hold))
                    {
                        if (InsufficientBreakAfterSustain(prevTarget,item))
                        {
                            errorLog.Add(new ErrorLogEntry(prevTarget.beatTime, "WARNING There's a very small gap between the end of this sustain target and the next target on the same hand; should have at least 1 beat of rest."));
                        }
                    }
                   
                    //short break after chain node
                    if (prevTarget.behavior.Equals(TargetBehavior.Chain) && !item.data.behavior.Equals(TargetBehavior.Chain))
                    {
                        if (InsufficientBreakAfterChain(prevTarget,item))
                        {
                            errorLog.Add(new ErrorLogEntry(prevTarget.beatTime, "WARNING There's a very small gap between the end of this chain and the next target on the same hand; should have at least 1 beat of rest."));
                        }
                    }

                    //headless chains
                    if (item.data.behavior.Equals(TargetBehavior.Chain) && !(prevTarget.behavior.Equals(TargetBehavior.Chain) || prevTarget.behavior.Equals(TargetBehavior.ChainStart)))
                    {
                        errorLog.Add(new ErrorLogEntry(item.data.beatTime, "ERROR This chain node does not have a proper Chain Start."));
                    }

                    // update prev target
                    if (item.data.handType.Equals(TargetHandType.Right)) { prevRHTarget = item.data; }
                    else { prevLHTarget = item.data; }

                }

                //Update previous target reference
                prevTarget = item.data;
            }


            // string output
            output = "Error checker has detected " + errorLog.Count + " errors/warnings." + System.Environment.NewLine;
            foreach(ErrorLogEntry item in errorLog)
            {
                output = output + "[" + (item.beatTime*480f) + "] " + item.errorDesc + System.Environment.NewLine;
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

        private bool InsufficientBreakAfterSustain(TargetData prevTarget, Target curTarget)
        {
            return curTarget.data.beatTime * 480 - (prevTarget.beatTime * 480 + prevTarget.beatLength) < 360;
        }

        private bool InsufficientBreakAfterChain(TargetData prevTarget, Target curTarget)
        {
            return curTarget.data.beatTime * 480 - prevTarget.beatTime * 480 < 360;
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
