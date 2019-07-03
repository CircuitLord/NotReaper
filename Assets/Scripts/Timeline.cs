using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NotReaper.IO;
using NotReaper.Models;
using NotReaper.Targets;
using UnityEngine;

namespace NotReaper {


    public class Timeline : MonoBehaviour {


        public GridTarget gridNotePrefab;
        public TimelineTarget timelineNotePrefab;

        [SerializeField] private Transform timelineTransformParent;
        [SerializeField] private Transform gridTransformParent;
        private OptionsMenu.DropdownToVelocity selectedSound;

        List<GridTarget> gridTargets;
        List<TimelineTarget> timelineTargets;
        public static List<GridTarget> orderedNotes;

        public static AudicaFile audicaFile;
        public TargetBehavior selectedTargetBehavior;
        public TargetHandType selectedTargetHandType;

        //The current time into the song.
        public static float time { get; private set; }
        private static int offset = 0;

        //The note colors.
        private Color leftColor;
        private Color rightColor;
        public Color bothColor;
        public Color neitherColor;


        private void Start() {
            leftColor = UserPrefsManager.leftColor;
            rightColor = UserPrefsManager.rightColor;
            bothColor = UserPrefsManager.bothColor;
            neitherColor = UserPrefsManager.leftColor;
        }


        //When the user adds a target, use this one.
        public void AddTarget(float x, float y) {
            AddTarget(x, y, GetBeatTime(), 1, TargetVelocity.Standard, selectedTargetHandType, selectedTargetBehavior, true);
        }

        //For adding targets to the grid and timeline. Used by both the user adding them and the loading of Audica Files.
        public void AddTarget(float x, float y, float beatTime, float beatLength = 0.25f, TargetVelocity velocity = TargetVelocity.Standard, TargetHandType handType = TargetHandType.Left, TargetBehavior behavior = TargetBehavior.Standard, bool userAdded = false) {

            // Add to timeline
            TimelineTarget timelineClone = Instantiate(timelineNotePrefab, timelineTransformParent);
            timelineClone.transform.localPosition = new Vector3(beatTime, 0, 0);

            // Add to grid
            GridTarget gridClone = Instantiate(gridNotePrefab, gridTransformParent);
            gridClone.GetComponentInChildren<HoldController>().length.text = "" + beatLength;
            gridClone.transform.localPosition = new Vector3(x, y, beatTime);

            gridClone.timelineTarget = timelineClone;
            timelineClone.timelineTarget = timelineClone;
            gridClone.gridTarget = gridClone;
            timelineClone.gridTarget = gridClone;

            //set velocity
            if (userAdded) {
                switch (selectedSound) {
                    case OptionsMenu.DropdownToVelocity.Standard:
                        gridClone.velocity = TargetVelocity.Standard;
                        break;

                    case OptionsMenu.DropdownToVelocity.Snare:
                        gridClone.velocity = TargetVelocity.Snare;
                        break;

                    case OptionsMenu.DropdownToVelocity.Percussion:
                        gridClone.velocity = TargetVelocity.Percussion;
                        break;

                    case OptionsMenu.DropdownToVelocity.ChainStart:
                        gridClone.velocity = TargetVelocity.ChainStart;
                        break;

                    case OptionsMenu.DropdownToVelocity.Chain:
                        gridClone.velocity = TargetVelocity.Chain;
                        break;

                    case OptionsMenu.DropdownToVelocity.Melee:
                        gridClone.velocity = TargetVelocity.Melee;
                        break;

                    default:
                        gridClone.velocity = velocity;
                        break;

                }
            } else {
                gridClone.SetVelocity(velocity);
            }

            gridClone.SetHandType(handType);
            gridClone.SetBehavior(behavior);

            if (gridClone.behavior == TargetBehavior.Hold)
                gridClone.SetBeatLength(beatLength);
            else
                gridClone.SetBeatLength(0.25f);

            gridTargets.Add(gridClone);
            timelineTargets.Add(timelineClone);

            orderedNotes = gridTargets.OrderBy(v => v.transform.position.z).ToList();

            UpdateTrail();

            UpdateChainConnectors();
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
            if (orderedNotes.Count > 0)
                foreach (var note in orderedNotes) {
                    if (note.behavior == TargetBehavior.ChainStart) {
                        LineRenderer lr = note.GetComponentsInChildren<LineRenderer>() [1];

                        List<Material> mats = new List<Material>();
                        lr.GetMaterials(mats);
                        Color activeColor = note.handType == TargetHandType.Left ? leftColor : rightColor;
                        mats[0].SetColor("_Color", activeColor);
                        mats[0].SetColor("_EmissionColor", activeColor);

                        //clear pos list
                        lr.SetPositions(new Vector3[2]);
                        lr.positionCount = 2;

                        //set start pos
                        lr.SetPosition(0, note.transform.position);
                        lr.SetPosition(1, note.transform.position);

                        //add links in chain
                        List<GridTarget> chainLinks = new List<GridTarget>();
                        foreach (var note2 in orderedNotes) {
                            //if new start end old chain
                            if ((note.handType == note2.handType) && (note2.transform.position.z > note.transform.position.z) && (note2.behavior == TargetBehavior.ChainStart) && (note2 != note))
                                break;

                            if ((note.handType == note2.handType) && note2.transform.position.z > note.transform.position.z && note2.behavior == TargetBehavior.Chain) {
                                chainLinks.Add(note2);
                            }

                        }

                        note.chainedNotes = chainLinks;

                        //aply lines to chain links
                        if (chainLinks.Count > 0) {
                            var positionList = new List<Vector3>();
                            positionList.Add(new Vector3(note.transform.position.x, note.transform.position.y, 0));
                            foreach (var link in chainLinks) {
                                //add new
                                if (!positionList.Contains(link.transform.position)) {
                                    positionList.Add(new Vector3(link.transform.position.x, link.transform.position.y, link.transform.position.z - note.transform.position.z));
                                }
                            }
                            lr.positionCount = positionList.Count;
                            positionList = positionList.OrderByDescending(v => v.z - note.transform.position.z).ToList();

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
            //TODO add all notes
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