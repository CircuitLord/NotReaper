using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NotReaper.Grid;
using NotReaper.IO;
using NotReaper.Models;
using NotReaper.Targets;
using UnityEngine;

namespace NotReaper {

    public class TimelineNew : MonoBehaviour {


        public GridTarget gridNotePrefab;
        public TimelineTarget timelineNotePrefab;

        [SerializeField] private Transform timelineTransformParent;
        [SerializeField] private Transform gridTransformParent;
        private OptionsMenu.DropdownToVelocity selectedSound;

        List<GridTarget> gridTargets;
        List<TimelineTarget> timelineTargets;
        //private List<Target> targets = new List<Target>();
        public static List<GridTarget> orderedTargets;

        public static AudicaFile audicaFile;
        public TargetBehavior selectedTargetBehavior;
        public TargetHandType selectedTargetHandType;

        //The current time into the song.
        public static float time { get; private set; }
        private static int offset = 0;

        //The note colors.
        private Color leftColor;
        private Color rightColor;
        private Color bothColor;
        private Color neitherColor;


        private void Start() {
            leftColor = UserPrefsManager.leftColor;
            rightColor = UserPrefsManager.rightColor;
            bothColor = UserPrefsManager.bothColor;
            neitherColor = UserPrefsManager.leftColor;


            orderedTargets = new List<GridTarget>();
            gridTargets = new List<GridTarget>();
            timelineTargets = new List<TimelineTarget>();
        }

        private void Update() {

            if (Input.GetKeyDown(KeyCode.P)) {
                LoadAudicaFile(@"C:\Files\GameStuff\AUDICACustom\Testing\test.audica");
            }

        }

        //When loading from cues, use this.
        public void AddTarget(Cue cue) {
            Vector2 pos = NotePosCalc.PitchToPos(cue);

            if (cue.tickLength / 480 >= 1)
                AddTarget(pos.x, pos.y, (cue.tick - offset) / 480f, cue.tickLength, cue.velocity, cue.handType, cue.behavior, false);
            else
                AddTarget(pos.x, pos.y, (cue.tick - offset) / 480f, 1, cue.velocity, cue.handType, cue.behavior, false);
        }

        //When the user adds a target, use this one.
        public void AddTarget(float x, float y) {
            AddTarget(x, y, GetBeatTime(), 1, TargetVelocity.Standard, selectedTargetHandType, selectedTargetBehavior, true);
        }

        //For adding targets to the grid and timeline. Used by both the user adding them and the loading of Audica Files.
        public void AddTarget(float x, float y, float beatTime, float beatLength = 0.25f, TargetVelocity velocity = TargetVelocity.Standard, TargetHandType handType = TargetHandType.Left, TargetBehavior behavior = TargetBehavior.Standard, bool userAdded = false) {

            // Add to timeline
            var timelineTarget = Instantiate(timelineNotePrefab, timelineTransformParent);
            timelineTarget.transform.localPosition = new Vector3(beatTime, 0, 0);

            // Add to grid
            var gridTarget = Instantiate(gridNotePrefab, gridTransformParent);
            gridTarget.GetComponentInChildren<HoldController>().length.text = "" + beatLength;
            gridTarget.transform.localPosition = new Vector3(x, y, beatTime);


            gridTarget.timelineTarget = timelineTarget;
            gridTarget.gridTarget = gridTarget;

            timelineTarget.timelineTarget = timelineTarget;
            timelineTarget.gridTarget = gridTarget;

            //set velocity
            if (userAdded) {
                switch (selectedSound) {
                    case OptionsMenu.DropdownToVelocity.Standard:
                        gridTarget.velocity = TargetVelocity.Standard;
                        break;

                    case OptionsMenu.DropdownToVelocity.Snare:
                        gridTarget.velocity = TargetVelocity.Snare;
                        break;

                    case OptionsMenu.DropdownToVelocity.Percussion:
                        gridTarget.velocity = TargetVelocity.Percussion;
                        break;

                    case OptionsMenu.DropdownToVelocity.ChainStart:
                        gridTarget.velocity = TargetVelocity.ChainStart;
                        break;

                    case OptionsMenu.DropdownToVelocity.Chain:
                        gridTarget.velocity = TargetVelocity.Chain;
                        break;

                    case OptionsMenu.DropdownToVelocity.Melee:
                        gridTarget.velocity = TargetVelocity.Melee;
                        break;

                    default:
                        gridTarget.velocity = velocity;
                        break;

                }
            } else {
                gridTarget.SetVelocity(velocity);
            }

            gridTarget.SetHandType(handType);
            gridTarget.SetBehavior(behavior);

            if (gridTarget.behavior == TargetBehavior.Hold)
                gridTarget.SetBeatLength(beatLength);
            else
                gridTarget.SetBeatLength(0.25f);

            gridTargets.Add(gridTarget);
            timelineTargets.Add(timelineTarget);

            orderedTargets = gridTargets.OrderBy(v => v.gridTarget.transform.position.z).ToList();

            UpdateTrail();

            //UpdateChainConnectors();

        }

        public void UpdateTrail() {
            Vector3[] positions = new Vector3[gridTransformParent.childCount];
            for (int i = 0; i < gridTransformParent.transform.childCount; i++) {
                positions[i] = gridTransformParent.GetChild(i).localPosition;
            }
            positions = positions.OrderBy(v => v.z).ToArray();
            var liner = gridTransformParent.gameObject.GetComponentInChildren<LineRenderer>();
            liner.positionCount = gridTransformParent.childCount;
            liner.SetPositions(positions);
        }

        private void UpdateChainConnectors() {

            if (orderedTargets.Count > 0)
                foreach (var note in orderedTargets) {
                    if (note.behavior == TargetBehavior.ChainStart) {
                        LineRenderer lr = note.gridTarget.GetComponentsInChildren<LineRenderer>() [1];

                        List<Material> mats = new List<Material>();
                        lr.GetMaterials(mats);
                        Color activeColor = note.handType == TargetHandType.Left ? leftColor : rightColor;
                        mats[0].SetColor("_Color", activeColor);
                        mats[0].SetColor("_EmissionColor", activeColor);

                        //clear pos list
                        lr.SetPositions(new Vector3[2]);
                        lr.positionCount = 2;

                        //set start pos
                        lr.SetPosition(0, note.gridTarget.transform.position);
                        lr.SetPosition(1, note.gridTarget.transform.position);

                        //add links in chain
                        List<GridTarget> chainLinks = new List<GridTarget>();
                        foreach (var note2 in orderedTargets) {
                            //if new start end old chain
                            if ((note.handType == note2.handType) && (note2.gridTarget.transform.position.z > note.gridTarget.transform.position.z) && (note2.behavior == TargetBehavior.ChainStart) && (note2 != note))
                                break;

                            if ((note.handType == note2.handType) && note2.gridTarget.transform.position.z > note.gridTarget.transform.position.z && note2.behavior == TargetBehavior.Chain) {
                                chainLinks.Add(note2);
                            }

                        }

                        note.chainedNotes = chainLinks;

                        //aply lines to chain links
                        if (chainLinks.Count > 0) {
                            var positionList = new List<Vector3>();
                            positionList.Add(new Vector3(note.gridTarget.transform.position.x, note.gridTarget.transform.position.y, 0));
                            foreach (var link in chainLinks) {
                                //add new
                                if (!positionList.Contains(link.gridTarget.transform.position)) {
                                    positionList.Add(new Vector3(link.gridTarget.transform.position.x, link.gridTarget.transform.position.y, link.gridTarget.transform.position.z - note.gridTarget.transform.position.z));
                                }
                            }
                            lr.positionCount = positionList.Count;
                            positionList = positionList.OrderByDescending(v => v.z - note.gridTarget.transform.position.z).ToList();

                            for (int i = 0; i < positionList.Count; ++i) {
                                positionList[i] = new Vector3(positionList[i].x, positionList[i].y, 0);
                            }

                            var finalPositions = positionList.ToArray();
                            lr.SetPositions(finalPositions);
                        }
                    }
                }


        }


        //Loads an audica file into the editor.
        private void LoadAudicaFile(string path) {
            audicaFile = AudicaHandler.LoadAudicaFile(path);

            //TODO: difficulty loading
            print(audicaFile);

            foreach (Cue cue in audicaFile.diffs.expert.cues) {
                AddTarget(cue);
            }


        }


        public static float GetBeatTime() {
            return DurationToBeats(time) - offset / 480f;
        }

        public static float DurationToBeats(float t) {
            return t * (float) audicaFile.desc.tempo / 60;
        }


        public void UpdateNoteColors() {
            leftColor = UserPrefsManager.leftColor;
            rightColor = UserPrefsManager.rightColor;
        }

    }
}