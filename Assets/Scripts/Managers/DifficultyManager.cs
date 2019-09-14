using System.Collections;
using System.Collections.Generic;
using NotReaper.Models;
using UnityEngine;

namespace NotReaper.Managers {


    public class DifficultyManager : MonoBehaviour {


        public bool existsExpert;
        public bool existsAdvanced;
        public bool existsStandard;
        public bool existsEasy;



        public Timeline timeline;

        //Use this when starting up, load highest diff in audica file
        public int LoadHighestDifficulty() {

            if (!Timeline.audicaLoaded) return -1;

            if (Timeline.audicaFile.diffs.expert.cues != null) {
                LoadDifficulty(0);
                return 0;  
            }

            else if (Timeline.audicaFile.diffs.advanced.cues != null) {
                LoadDifficulty(1);
                return 1;  
            }

            else if (Timeline.audicaFile.diffs.standard.cues != null) {
                LoadDifficulty(2);
                return 2;  
            }

            else if (Timeline.audicaFile.diffs.easy.cues != null) {
                LoadDifficulty(3);
                return 3;  
            }

            //If no difficulties exist gen a new one
            bool genned = GenerateDifficulty(0);
            if (genned) return 0;
            
            else return -1;
            

        }

        /// <summary>
        /// Overrides any existing difficulty with a blank cues object.
        /// </summary>
        /// <param name="index">The index of the difficulty to clear/generate.0-expert, 3-easy</param>
        /// <returns></returns>
        public bool GenerateDifficulty(int index) {

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

        public bool RemoveDifficulty(int index) {

        }


        public bool LoadDifficulty(int index) {

            if (!Timeline.audicaLoaded) return false;

            switch (index) {
                case 0:
                    if (Timeline.audicaFile.diffs.expert.cues != null) {

                    }
                    break;
            }



        }


    }

}