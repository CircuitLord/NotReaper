using System.Collections;
using System.Collections.Generic;
using NotReaper.Models;
using TMPro;
using UnityEngine;

namespace NotReaper.Managers {


    public class DifficultyManager : MonoBehaviour {


        [HideInInspector] public int loadedIndex = -1;

        [SerializeField] private NRDiscordPresence nrDiscordPresence;

        [SerializeField] private TextMeshProUGUI curSongName;
		[SerializeField] private TextMeshProUGUI curSongDiff;
        



        public Timeline timeline;

        //Use this when starting up, load highest diff in audica file
        public int LoadHighestDifficulty(bool save = false) {

            if (!Timeline.audicaLoaded) return -1;

            if (Timeline.audicaFile.diffs.expert.cues != null) {
                LoadDifficulty(0, save);
                
                return 0;  
            }

            else if (Timeline.audicaFile.diffs.advanced.cues != null) {
                LoadDifficulty(1, save);
                return 1;  
            }

            else if (Timeline.audicaFile.diffs.standard.cues != null) {
                LoadDifficulty(2, save);
                return 2;  
            }

            else if (Timeline.audicaFile.diffs.easy.cues != null) {
                LoadDifficulty(3, save);
                return 3;  
            }

            //If no difficulties exist gen a new one
            bool genned = GenerateDifficulty(0);
            if (genned) return 0;

            else return -1;
        }

        /// <summary>
        /// Overrides any difficulty with a blank cues object.
        /// </summary>
        /// <param name="index">The index of the difficulty to clear/generate.0-expert, 3-easy</param>
        /// <returns>True if succeeded</returns>
        public bool GenerateDifficulty(int index) {

            if (DifficultyExists(index)) return false;

            switch (index) {
                case 0:
                    Timeline.audicaFile.diffs.expert.cues = new List<Cue>();
                    break;
                case 1:
                    Timeline.audicaFile.diffs.advanced.cues = new List<Cue>();
                    break;
                case 2:
                    Timeline.audicaFile.diffs.standard.cues = new List<Cue>();
                    break;
                case 3:
                    Timeline.audicaFile.diffs.easy.cues = new List<Cue>();
                    break;
                
            }
            return true;
        }

        public void RemoveDifficulty(int index) {
            if (loadedIndex == index) timeline.DeleteAllTargets();
            switch (index) {
                case 0:
                    Timeline.audicaFile.diffs.expert.cues = null;
                    break;
                case 1:
                    Timeline.audicaFile.diffs.advanced.cues = null;
                    break;
                case 2:
                    Timeline.audicaFile.diffs.standard.cues = null;
                    break;
                case 3:
                    Timeline.audicaFile.diffs.easy.cues = null;
                    break;
                
            }

        }

        public bool DifficultyExists(int index) {
            DiffsList diffs = Timeline.audicaFile.diffs;
            switch (index) {
                case 0:
                    if (diffs.expert.cues != null) {
                        return true;
                    }
                    break;
                case 1:
                    if (diffs.advanced.cues != null) {
                        return true;
                    }
                    break;
                case 2:
                    if (diffs.standard.cues != null) {
                        return true;
                    }
                    break;
                case 3:
                    if (diffs.easy.cues != null) {
                        return true;
                    }
                    break;
            }
            //if it fell through
            return false;
        }

        public bool CopyToOtherDifficulty(int origin, int dest) {

            if (!DifficultyExists(origin)) return false;

            //Save the current difficulty
            timeline.Export();

            DiffsList diffs = Timeline.audicaFile.diffs;

            switch (origin) {
                case 0:
                    ActuallyCopyToOtherDifficulty(diffs.expert.cues, dest);
                    break;
                case 1:
                    ActuallyCopyToOtherDifficulty(diffs.advanced.cues, dest);
                    break;
                case 2:
                    ActuallyCopyToOtherDifficulty(diffs.standard.cues, dest);
                    break;
                case 3:
                    ActuallyCopyToOtherDifficulty(diffs.easy.cues, dest);
                    break;
            }

            return true;


        }

        private void ActuallyCopyToOtherDifficulty(List<Cue> cues, int dest) {
            switch (dest) {
                case 0:
                    Timeline.audicaFile.diffs.expert.cues = cues;
                    break;
                case 1:
                    Timeline.audicaFile.diffs.advanced.cues = cues;
                    break;
                case 2:
                    Timeline.audicaFile.diffs.standard.cues = cues;
                    break;
                case 3:
                    Timeline.audicaFile.diffs.easy.cues = cues;
                    break;
            }
        }


        public bool LoadDifficulty(int index, bool save = true) {


            if (!Timeline.audicaLoaded) return false;

            DiffsList diffs = Timeline.audicaFile.diffs;
            curSongName.text = Timeline.desc.title;

            Debug.Log("Loading diff: " + index);
            switch (index) {
                case 0:
                    if (diffs.expert.cues != null) {
                        curSongDiff.text = "Expert";
                        LoadTimelineDiff(diffs.expert.cues, save);
                        loadedIndex = index;

                        nrDiscordPresence.UpdatePresenceDifficulty(0);
                        return true;
                    }
                    break;
                case 1:
                    if (diffs.advanced.cues != null) {
                        curSongDiff.text = "Advanced";
                        LoadTimelineDiff(diffs.advanced.cues, save);
                        loadedIndex = index;

                        nrDiscordPresence.UpdatePresenceDifficulty(1);
                        return true;
                    }
                    break;
                case 2:
                    if (diffs.standard.cues != null) {
                        curSongDiff.text = "Standard";
                        LoadTimelineDiff(diffs.standard.cues, save);
                        loadedIndex = index;

                        nrDiscordPresence.UpdatePresenceDifficulty(2);
                        return true;
                    }
                    break;
                case 3:
                    if (diffs.easy.cues != null) {
                        curSongDiff.text = "Easy";
                        LoadTimelineDiff(diffs.easy.cues, save);
                        loadedIndex = index;

                        nrDiscordPresence.UpdatePresenceDifficulty(3);
                        return true;
                    }
                    break;
            }

            //Else, if it failed, return false
            return false;



        }

        private bool LoadTimelineDiff(List<Cue> cues, bool save = true) {

            if (save) timeline.Export();

            timeline.DeleteAllTargets();

            foreach (Cue cue in cues) {
                timeline.AddTarget(cue);
            }

            return true;
        }


    }

}