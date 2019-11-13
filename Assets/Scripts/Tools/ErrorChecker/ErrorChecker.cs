using NotReaper.IO;
using NotReaper.Managers;
using NotReaper.Models;
using NotReaper.Targets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NotReaper.Timing;

namespace NotReaper.Tools.ErrorChecker
{

    public class ErrorChecker : MonoBehaviour
    {

        //Hidden public values
        //[HideInInspector] public static AudicaFile audicaFile;

        [SerializeField] private DifficultyManager difficultyManager;


        private List<ErrorLogEntry> currentErrors = new List<ErrorLogEntry>();
        private int currentErrorIndex = -1;
        private ErrorLogEntry currentError;

        public Timeline timeline;

        public GameObject errorCheckUI;
        public TextMeshProUGUI errorCountText;
        public TextMeshProUGUI errorBodyText;

        private QNT_Duration chainLeadTime = new QNT_Duration(360);
        private QNT_Duration sustainLeadTime = new QNT_Duration(360);

        private void Start() {
	        errorCheckUI.SetActive(false);
        }

        public void RunErrorCheck()
        {
            /* retrieve orderedNotes
             * parse notes for errors
             * export error messages to txt
             */
            
            currentErrors.Clear();

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
            //string outputMessage = "";
            currentErrors = ParseCues(notes, "", difficulty, difficultyLabel);

            //export
            //output to txt
            //System.IO.File.WriteAllText(@"D:\test\test.txt", outputMessage);
            
            EnableErrorCheckingUI();

            UpdateErrorCount();

            currentErrorIndex = -1;
            
            NextError();
        }


        public void EnableErrorCheckingUI() {
	        errorCheckUI.SetActive(true);
        }

        public void DisableErrorCheckingUI() {
	        errorCheckUI.SetActive(false);
        }

        public void NextError() {
	        
	        //Deselect any previous targets
	        if (currentError != null) {
		        foreach (Target target in currentError.affectedTargets) {
			        target.Deselect();
		        }
	        }
	        
	        
	        if (currentErrors.Count <= 0) return;

	        if (currentErrorIndex >= currentErrors.Count - 1) return;

	        currentErrorIndex++;

	        currentError = currentErrors[currentErrorIndex];

	        if (currentError == null) return;
	        
	        errorBodyText.SetText(currentError.errorDesc);
	        
	        if (!timeline.paused) timeline.TogglePlayback();
	        //timeline.JumpToX(currentError.beatTime);
	        
	        StartCoroutine(timeline.AnimateSetTime(currentError.time));
	        
	        
	        //Select the targets

	        foreach (Target target in currentError.affectedTargets) {
		        target.Select();
	        }




        }

        public void PrevError() {

	        if (currentErrors.Count <= 0) return;
	        
	        if (currentErrorIndex <= 0) return;

	        currentErrorIndex--;

	        currentError = currentErrors[currentErrorIndex];

	        if (currentError == null) return;
	        
	        errorBodyText.SetText(currentError.errorDesc);
	        
	        if (!timeline.paused) timeline.TogglePlayback();

	       // timeline.SetBeatTime(time);
	       StartCoroutine(timeline.AnimateSetTime(currentError.time));

        }

        public void MarkCurrentFixed() {
	        currentErrors.Remove(currentError);
	        currentError = null;
	        NextError();
	        UpdateErrorCount();
        }

        public void UpdateErrorCount() {
	        errorCountText.SetText("Errors: " + currentErrors.Count);
        }
        
        
        
        
        
        

        private List<ErrorLogEntry> ParseCues(List<Target> targetCues, string output, int difficulty, string label)
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
            
            
            
            
            //Check for a preview point:
            if (Timeline.audicaFile.desc.previewStartSeconds == 0) {
	            errorLog.Add(new ErrorLogEntry(new QNT_Timestamp(0), "No preview start point has been added. Go to a point in the song and press P to set it."));
            }
            

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
                    var error = new ErrorLogEntry(curTarget.data.time, "ERROR, target has an invalid hitsound.");
                    
                    error.affectedTargets.Add(curTarget);
                    errorLog.Add(error);
                }

                //////////////////////////////
                // general backtrack checks //
                //////////////////////////////

                // consecutive rhythm check
                // if lower difficulties and not chain node
                if (difficulty != 0 && (!prevTarget.behavior.Equals(TargetBehavior.Chain) && !curTarget.data.behavior.Equals(TargetBehavior.Chain)))
                {
                    QNT_Duration rhythmLimit = SetRhythmLimit(difficulty);
                    int countLimit = SetCountLimit(difficulty);
                    QNT_Duration beatTimeDiff = new QNT_Duration(curTarget.data.time.tick - prevTarget.time.tick);
                    if (beatTimeDiff.tick > rhythmLimit.tick)
                    {
                        //reset counter
                        consecutiveCounter = 0;
                    }
                    else if(beatTimeDiff != 0 && beatTimeDiff < rhythmLimit)
                    {
                        //straight up too fast
                        var error = new ErrorLogEntry(curTarget.data.time, "WARNING, for " + label + ", this target happens too soon after the previous target.");
                        error.affectedTargets.Add(curTarget);
                        errorLog.Add(error);
                    }
                    else if(beatTimeDiff == rhythmLimit)
                    {
                        // consecutive 8th notes on one hand
                        if(difficulty==2 && prevTarget.handType.Equals(curTarget.data.handType))
                        {
                            Debug.Log("consecutive 8t notes on one hand");
                            var error = new ErrorLogEntry(curTarget.data.time, "WARNING, in " + label + ", consecutive 8th notes on one hand are not recommended.");
                            error.affectedTargets.Add(curTarget);
                            errorLog.Add(error);
                        }
                        
                        //increment counter, if it gets above the countLimit, log an error
                        consecutiveCounter++;

                        if(consecutiveCounter >= countLimit)
                        {
                            //TODO convert rhythmLimit to quarter note, eigth note, etc.
                            var error = new ErrorLogEntry(curTarget.data.time, "WARNING, in " + label + ", having more than " + countLimit + " consecutive " + rhythmLimit + " targets is not recommended.");
                            
                            error.affectedTargets.Add(curTarget);
                            errorLog.Add(error);
                        }
                    }

                }

                //simultaneous target checks
                //chains, melees, and pathbuilder notes don't count
                if (
                    prevTarget.time == curTarget.data.time && 
                    (!prevTarget.behavior.Equals(TargetBehavior.Chain) && !curTarget.data.behavior.Equals(TargetBehavior.Chain)) && 
                    (!prevTarget.behavior.Equals(TargetBehavior.Melee) && !curTarget.data.behavior.Equals(TargetBehavior.Melee)) &&
                    (!prevTarget.behavior.Equals(TargetBehavior.NR_Pathbuilder) && !curTarget.data.behavior.Equals(TargetBehavior.NR_Pathbuilder))
                )

                {
                    // same pitch
                    if (prevTarget.position == curTarget.data.position)
                    {
                        var error = new ErrorLogEntry(prevTarget.time, "ERROR, there are multiple targets occupying the same position.");
                        error.affectedTargets.Add(timeline.FindNote(prevTarget));
                        errorLog.Add(error);
                    }

                    // same color
                    if (prevTarget.handType.Equals(curTarget.data.handType) && !prevTarget.handType.Equals(TargetHandType.Either)) {
	                    var error = new ErrorLogEntry(prevTarget.time,
		                    "ERROR, the " + prevTarget.handType + " hand has multiple targets at the same time.");
	                    
	                    error.affectedTargets.Add(timeline.FindNote(prevTarget));
	                    errorLog.Add(error);
                    }

                    // ADVANCED and lower
                    if (difficulty > 0)
                    {
                        
                        //simultaneous shot and melee
                        if (IsSimultaneousShotAndMelee(prevTarget,curTarget))
                        {
                            var error = new ErrorLogEntry(prevTarget.time, "WARNING, in " + label + ", simultaneous melee and shot targets are not recommended.");
                            error.affectedTargets.Add(timeline.FindNote(prevTarget));
                            errorLog.Add(error);
                        }
                        //simultaneous targets must be within 4 spaces apart for Advanced, 3 for Standard/Beginner
                        else
                        {
                            float distance = (difficulty == 1 ? 4 : 3);
                            if (!IsCloseEnough(prevTarget, curTarget, distance))
                            {
                                var error = new ErrorLogEntry(prevTarget.time, "WARNING, in " + label + ", simultaneous targets more than " + distance + " spaces apart are not recommended.");
                                error.affectedTargets.Add(timeline.FindNote(prevTarget));
                                errorLog.Add(error);
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
                        if (!IsSlottedNote(prevTarget) && InsufficientBreakAfterPreviousTarget(prevTarget, curTarget, new QNT_Duration(Constants.PulsesPerQuarterNote * 2))) {
	                        var error = new ErrorLogEntry(prevTarget.time,
		                        "WARNING, in ADVANCED, it is recommended to have at least 2 beats of lead-in time before introducing a horizontal/vertical slotted note.");
	                        
	                        error.affectedTargets.Add(timeline.FindNote(prevTarget));
	                        errorLog.Add(error);
                        }
                    }
                    else // no slotted notes for STANDARD or BEGINNER
                    {
	                    var error = new ErrorLogEntry(prevTarget.time,
		                    "WARNING, in " + label + ", use of horizontal/vertical slotted notes is not recommended.");
	                    
	                    error.affectedTargets.Add(timeline.FindNote(prevTarget));
	                    errorLog.Add(error);
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
                        var error = new ErrorLogEntry(prevTarget.time, "WARNING, this Melee Target is by itself in the lower slot. Single melees should be in the higher slot. Only use the lower slot for making simultaneous melees stacked on top of each other.");
                        error.affectedTargets.Add(timeline.FindNote(prevTarget));
                        errorLog.Add(error);
                    }
                }

                if (curTarget.data.behavior.Equals(TargetBehavior.Melee)) {
                    //non melee hitsound
                    if (!IsMeleeHitSound(curTarget))
                    {
                        var error = new ErrorLogEntry(curTarget.data.time, "WARNING, Melee Target doesn't have a Melee hitsound.");
                        error.affectedTargets.Add(curTarget);
                        errorLog.Add(error);
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
                            var error = new ErrorLogEntry(prevTarget.time, "WARNING, the time between the end of this sustain target and the next target on the same hand is very short; at least " + sustainLeadTime + " is recommended.");
                            error.affectedTargets.Add(curTarget);
                            errorLog.Add(error);
                        }
                    }
                   
                    //short break after chain node
                    if (prevTarget.behavior.Equals(TargetBehavior.Chain) && !curTarget.data.behavior.Equals(TargetBehavior.Chain))
                    {
                        if (InsufficientBreakAfterPreviousTarget(prevTarget,curTarget,chainLeadTime)) {
	                        var error = new ErrorLogEntry(prevTarget.time,
		                        "WARNING, the time between the end of this chain and the next target on the same hand is very short; at least " +
		                        chainLeadTime + " is recommended.");
	                        
	                        error.affectedTargets.Add(curTarget);
	                        errorLog.Add(error);
                        }
                    }

                    //headless chains
                    if (curTarget.data.behavior.Equals(TargetBehavior.Chain) && !(prevTarget.behavior.Equals(TargetBehavior.Chain) || prevTarget.behavior.Equals(TargetBehavior.ChainStart)))
                    {
                        var error = new ErrorLogEntry(curTarget.data.time, "ERROR, this chain node does not have a proper Chain Start.");
                        error.affectedTargets.Add(curTarget);
                        errorLog.Add(error);
                    }

                    // update prev target
                    if (curTarget.data.handType.Equals(TargetHandType.Right)) { prevRHTarget = curTarget.data; }
                    else { prevLHTarget = curTarget.data; }

                }

                //Update previous target reference
                prevTarget = curTarget.data;
            }


            // string output
            Debug.Log("Error checker has detected " + errorLog.Count + " errors/warnings.");
          //  foreach(ErrorLogEntry item in errorLog)
           // {
                //output = output + "[" + (item.beatTime*TickBeatConst) + "] " + item.errorDesc + System.Environment.NewLine;
           //}
            return errorLog;
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
            if (prev.behavior.Equals(TargetBehavior.Melee) && prev.time == lm.time && prev.position.x * lm.position.x > 0)
            { return false; }
            if (next.behavior.Equals(TargetBehavior.Melee) && next.time == lm.time && next.position.x * lm.position.x > 0)
            { return false; }
                return true;
        }

        private bool InsufficientBreakAfterSustain(TargetData prevTarget, Target curTarget, QNT_Duration leadTime)
        {
            return curTarget.data.time.tick - (prevTarget.time.tick + prevTarget.beatLength.tick) < leadTime.tick;
        }

        private bool InsufficientBreakAfterPreviousTarget(TargetData prevTarget, Target curTarget, QNT_Duration leadTime)
        {
            return curTarget.data.time.tick - prevTarget.time.tick < leadTime.tick;
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

        private QNT_Duration SetRhythmLimit(int difficulty)
        {
            QNT_Duration limit = Constants.QuarterNoteDuration;
            switch (difficulty)
            {
                case 1:
                    limit = new QNT_Duration(Constants.PulsesPerQuarterNote);
                    break;
                case 2:
                    limit = new QNT_Duration(Constants.PulsesPerQuarterNote * 2);
                    break;
                case 3:
                    limit = new QNT_Duration(Constants.PulsesPerQuarterNote * 4);
                    break;
                default:
                    limit = new QNT_Duration(Constants.PulsesPerQuarterNote);
                    break;
            }
            return limit;
        }


    }
}
