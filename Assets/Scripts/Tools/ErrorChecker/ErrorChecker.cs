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
            /* retrieve loadedNotes
             * parse notes for errors
             * export error messages to txt
             */

            //retrieve orderedNotes and difficulty label
            List<Target> notes = Timeline.orderedNotes;
            int difficulty = difficultyManager.loadedIndex;
            //CueFile cuesFile = new CueFile();
            //cuesFile.cues = new List<Cue>();

            //switch (difficultyManager.loadedIndex)
            //{
            //    case 0:
            //        cuesFile = audicaFile.diffs.expert;
            //        break;
            //    case 1:
            //        cuesFile = audicaFile.diffs.advanced;
            //        break;
            //    case 2:
            //        cuesFile = audicaFile.diffs.moderate;
            //        break;
            //    case 3:
            //        cuesFile = audicaFile.diffs.beginner;
            //        break;
            //}

            //parse
            //string cuesFileJson = "";
            string outputMessage = "";
            //cuesFileJson = AudicaExporter.CuesToJson(cuesFile);
            outputMessage = ParseCues(notes, outputMessage);

            //export output to txt
            System.IO.File.WriteAllText(@"D:\test\test.txt", outputMessage);
        }

        private string ParseCues(List<Target> targetCues, string output)
        {
            List<ErrorLogEntry> errorLog = new List<ErrorLogEntry>();
            Target prevTarget;
            Target prevRHTarget;
            Target prevLHTarget;
            Target prevMeleeTarget;

            foreach (Target item in targetCues)
            {
                

                //check for
                //cues without hitsounds
                if (!HasHitSound(item))
                {
                    errorLog.Add(new ErrorLogEntry(item.data.beatTime, "ERROR Target has an invalid hitsound."));
                }

                //invalid tickLength
                if (!HasTickLength(item))
                {
                    errorLog.Add(new ErrorLogEntry(item.data.beatTime, "WARNING Target has 0 tickLength. (Map was made using on older version of the editor. Please update to latest version)"));
                }

                //melee checks
                if (item.data.behavior.Equals(TargetBehavior.Melee)) {
                    Debug.Log("There's a melee here, it's at: " + item.data.position + System.Environment.NewLine + "beatTime: " + item.data.beatTime*480f);
                    //Debug.Log("beatTime: " + item.data.beatTime);
                    //non melee hitsound
                    if (!IsMeleeHitSound(item))
                    {
                        errorLog.Add(new ErrorLogEntry(item.data.beatTime, "WARNING Melee Target doesn't have a Melee hitsound."));
                    }

                    //low solo melee
                    //if(IsLowSoloMelee(item))
                    //{
                    //    errorLog.Add(new ErrorLogEntry(item.data.beatTime, "WARNING Melee Target is in the lower slot. Melees should be in the higher slot, and only use the lower slot for simultaneous Melees."));
                    //}

                    //update previous melee reference
                    prevMeleeTarget = item;
                }

                //Update previous target reference
                prevTarget = item;
            }

            output = "Error checker has detected " + errorLog.Count + " errors/warnings." + System.Environment.NewLine;
            foreach(ErrorLogEntry item in errorLog)
            {
                output = output + "[" + (item.beatTime*480f) + "] " + item.errorDesc + System.Environment.NewLine;
            }
            return output;
            /*TODO
             * headless chains
             * targets at same pitch at same time
             * same color targets at same time
             */
        }

        //checker functions

        private bool HasHitSound(Target targetCue)
        {
            if (Enum.IsDefined(typeof(TargetVelocity), targetCue.data.velocity))
            { return true; }
            else { return false; }
        }

        private bool HasTickLength(Target targetCue)
        {
            if (targetCue.data.beatLength!=0)
            { return true; }
            else { return false; }
        }

        private bool IsMeleeHitSound(Target targetCue)
        {
            //Debug.Log("Data velocity is: " + targetCue.data.velocity)


            if (targetCue.data.velocity.Equals(TargetVelocity.Melee))
            { return true; }
            else { return false; }
        }

        private bool IsLowSoloMelee(Target targetCue)
        {
            //check if low
            //TODO and check if solo
            if (targetCue.data.position.y < 0)
            { return true; }
            else { return false; }
        }

        /*
        public static string CuesToJson(CueFile cueFile)
        {
            return JsonUtility.ToJson(cueFile, true);
        }*/

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
