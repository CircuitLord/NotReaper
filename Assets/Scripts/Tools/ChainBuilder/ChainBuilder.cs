using System;
using System.Collections.Generic;
using System.Linq;
using NotReaper.Grid;
using NotReaper.Models;
using NotReaper.Targets;
using NotReaper.UserInput;
using UnityEngine;
using DG.Tweening;
using NotReaper.Timing;

namespace NotReaper.Tools.ChainBuilder { 

	public static class Vector2Extension {
     public static Vector2 Rotate(this Vector2 v, float degrees) {
         float radians = degrees * Mathf.Deg2Rad;
         float sin = Mathf.Sin(radians);
         float cos = Mathf.Cos(radians);
         
         float tx = v.x;
         float ty = v.y;
 
         return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
     }
 }
 

	public class ChainBuilder : MonoBehaviour {
		public static Timeline timeline;
		public LayerMask notesLayer;

		private TargetIcon[] _iconsUnderMouse = new TargetIcon[0];

		/** Fetch all icons currently under the mouse
		 *  Will only ever happen once per frame */
		public TargetIcon[] iconsUnderMouse {
			get {
				return _iconsUnderMouse = _iconsUnderMouse == null
					? MouseUtil.IconsUnderMouse(timeline)
					: _iconsUnderMouse;
			}
			set { _iconsUnderMouse = value; }
		}

		// Fetch the highest priority target (closest to current time)
		public TargetIcon iconUnderMouse {
			get {
				return iconsUnderMouse != null && iconsUnderMouse.Length > 0
					? iconsUnderMouse[0]
					: null;
			}
		}


		public GameObject curvedLinePrefab;
		public GameObject linePointPrefab;
		public GameObject nodeTempPointPrefab;

		[SerializeField] private Transform chainBuilderLinesParent;
		private Transform tempNodeIconsParent;

		public LayerMask chainBuilderLayer;

		public bool isDragging = false;
		public bool isEditMode = false;
		public bool activated = false;
		public bool isHovering = false;

		private Transform draggingPoint;
		public GameObject activeChain;

		private int prevDrawPointsAmt = 0;

		private Target startClickNote = null;



		[SerializeField] private GameObject chainBuilderWindow; 
		[SerializeField] private GameObject chainBuilderWindowSelectedControls; 
		[SerializeField] private GameObject chainBuilderWindowUnselectedControls; 

		[SerializeField] private Michsky.UI.ModernUIPack.HorizontalSelector pathBuilderInterval;
		[SerializeField] private TextSliderCombo angleIncrement;
		[SerializeField] private TextSliderCombo angleIncrementIncrement;
		[SerializeField] private TextSliderCombo stepDistance;
		[SerializeField] private TextSliderCombo stepIncrement;

		

		void Start() {
			Vector3 defaultPos;
			defaultPos.x = 289.0f;
			defaultPos.y = -92.2f;
			defaultPos.z = -10.0f;

			chainBuilderWindow.GetComponent<RectTransform>().localPosition = defaultPos;
			chainBuilderWindow.GetComponent<CanvasGroup>().alpha = 0.0f;

			angleIncrement.OnValueChanged += OnAngleVelocityChange;
			angleIncrementIncrement.OnValueChanged += OnAngleAccelerationChange;
			stepDistance.OnValueChanged += OnStepDistanceChange;
			stepIncrement.OnValueChanged += OnStepIncrementChange;
		}

		/// <summary>
		/// Sets if the tool is active or not.
		/// </summary>
		/// <param name="active"></param>
		public void Activate(bool active) {
			bool wasActive = activated == true;

			isHovering = false;
			activated = active;

			startClickNote = null;

			if(active) {
				bool validNoteSelected = (timeline.selectedNotes.Count == 1 && timeline.selectedNotes[0].data.behavior == TargetBehavior.NR_Pathbuilder);

				if(!validNoteSelected) {
					timeline.DeselectAllTargets();
				}

				chainBuilderWindow.GetComponent<CanvasGroup>().DOFade(1.0f, 0.3f);
				chainBuilderWindow.SetActive(true);

				if(validNoteSelected) {
					SetPathbuilderStateToSelectedNote();
				}
			}
			else {
				chainBuilderWindow.GetComponent<CanvasGroup>().DOFade(0.0f, 0.3f);
				chainBuilderWindow.SetActive(false);
			}

			if(wasActive && active == false) {
				List<TargetData> nonGeneratedNotes = new List<TargetData>();

				foreach(Target note in timeline.notes) {
					if(note.data.behavior == TargetBehavior.NR_Pathbuilder && note.data.pathBuilderData.createdNotes == false) {
						nonGeneratedNotes.Add(note.data);
					}
				}

				foreach(var data in nonGeneratedNotes) {
					GenerateChainNotes(data);
				}
			}
		}

		public void OnIntervalChange() {
			Target target = timeline.selectedNotes.First();
			if(target == null || target.data.behavior != TargetBehavior.NR_Pathbuilder) {
				return;
			}

			string temp = pathBuilderInterval.elements[pathBuilderInterval.index];
			int snap = 4;
			int.TryParse(temp.Substring(2), out snap);

			angleIncrement.value = target.data.pathBuilderData.angle;
			stepDistance.value = target.data.pathBuilderData.stepDistance;

			target.data.pathBuilderData.interval = snap;
		}

		public void OnAngleVelocityChange(float value) {
			Target target = timeline.selectedNotes.First();
			if(target == null || target.data.behavior != TargetBehavior.NR_Pathbuilder) {
				return;
			}

			target.data.pathBuilderData.angle = value;
		}

		public void OnAngleAccelerationChange(float value) {
			Target target = timeline.selectedNotes.First();
			if(target == null || target.data.behavior != TargetBehavior.NR_Pathbuilder) {
				return;
			}

			target.data.pathBuilderData.angleIncrement = value;
		}

		public void OnStepDistanceChange(float value) {
			Target target = timeline.selectedNotes.First();
			if(target == null || target.data.behavior != TargetBehavior.NR_Pathbuilder) {
				return;
			}

			target.data.pathBuilderData.stepDistance = value;
		}

		public void OnStepIncrementChange(float value) {
			Target target = timeline.selectedNotes.First();
			if(target == null || target.data.behavior != TargetBehavior.NR_Pathbuilder) {
				return;
			}

			target.data.pathBuilderData.stepIncrement = value;
		}

		public void GeneratePathFromSelectedNote() {
			Target target = timeline.selectedNotes.First();
			if(target == null || target.data.behavior != TargetBehavior.NR_Pathbuilder) {
				return;
			}
			GenerateChainNotes(target.data);
		}

		public static void GenerateChainNotes(TargetData data) {
			CalculateChainNotes(data);

			//Add new notes
			data.pathBuilderData.generatedNotes.ForEach(t => {
				var newTarget = timeline.AddTargetFromAction(t, true);
			});

			data.pathBuilderData.createdNotes = true;
		}
		
		public static void CalculateChainNotes(TargetData parentData) {
			if(parentData.behavior != TargetBehavior.NR_Pathbuilder) {
				return;
			}

			if(parentData.pathBuilderData.createdNotes) {
                parentData.pathBuilderData.generatedNotes.ForEach(t => {
					timeline.DeleteTargetFromAction(t);
				});
                parentData.pathBuilderData.createdNotes = false;
			}

			//No notes can be generated
			if(parentData.beatLength.tick == 0) {
				return;
			}

            parentData.pathBuilderData.generatedNotes = new List<TargetData>();

            foreach (TargetData data in parentData.pathBuilderData.parentNotes) {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /////////                                            WARNING!                                                      /////////
                /////////       Chainging this calculation breaks backwards compatibility with saves of older NotReaper versions!  /////////
                /////////                    Make sure to update NRCueData.Version, and handle an upgrade path!                    /////////
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //Generate first note at the start
                TargetData firstData = new TargetData();
                firstData.behavior = data.pathBuilderData.behavior;
                firstData.velocity = data.pathBuilderData.velocity;
                firstData.handType = data.pathBuilderData.handType;

                //Force set the time, since these transient notes will get generated for all pathbuilders in repeaters
                firstData.SetTimeFromAction(data.time);

                firstData.position = data.position;
                data.pathBuilderData.generatedNotes.Add(firstData);

                //We increment as if all these values were for 1/4 notes over 4 beats, makes the ui much better
                float quarterIncrConvert = (4.0f / data.pathBuilderData.interval) * (Constants.PulsesPerQuarterNote * 4.0f / data.beatLength.tick);

                //Generate new notes
                Vector2 currentPos = data.position;
                Vector2 currentDir = new Vector2(Mathf.Sin(data.pathBuilderData.initialAngle * Mathf.Deg2Rad), Mathf.Cos(data.pathBuilderData.initialAngle * Mathf.Deg2Rad));
                float currentAngle = (data.pathBuilderData.angle / 4) * quarterIncrConvert;
                float currentStep = data.pathBuilderData.stepDistance * quarterIncrConvert;

                TargetBehavior generatedBehavior = data.pathBuilderData.behavior;
                if (generatedBehavior == TargetBehavior.ChainStart) {
                    generatedBehavior = TargetBehavior.Chain;
                }

                TargetVelocity generatedVelocity = data.pathBuilderData.velocity;
                if (generatedVelocity == TargetVelocity.ChainStart) {
                    generatedVelocity = TargetVelocity.Chain;
                }

                for (int i = 1; i <= (data.beatLength.tick / (float)Constants.PulsesPerQuarterNote) * (data.pathBuilderData.interval / 4.0f); ++i) {
                    currentPos += currentDir * currentStep;
                    currentDir = currentDir.Rotate(currentAngle);

                    currentAngle += (data.pathBuilderData.angleIncrement / 4) * quarterIncrConvert;
                    currentStep += data.pathBuilderData.stepIncrement * quarterIncrConvert;

                    TargetData newData = new TargetData();
                    newData.behavior = generatedBehavior;
                    newData.velocity = generatedVelocity;
                    newData.handType = data.pathBuilderData.handType;

                    //Force set the time, since these transient notes will get generated for all pathbuilders in repeaters
                    newData.SetTimeFromAction(data.time + QNT_Duration.FromBeatTime(i * (4.0f / data.pathBuilderData.interval)));

                    newData.position = currentPos;
                    data.pathBuilderData.generatedNotes.Add(newData);
                }
            }

			parentData.pathBuilderData.OnFinishRecalculate();
		}

		public void BakePathFromSelectedNote() {
			Target target = timeline.selectedNotes.First();
			if(target == null || target.data.behavior != TargetBehavior.NR_Pathbuilder) {
				return;
			}

			NRActionBakePath action = new NRActionBakePath();
            action.removeNoteAction = new NRActionRemoveNote { targetData = target.data };
			timeline.Tools.undoRedoManager.AddAction(action);
		}

		/// <summary>
		/// Sets the tool to be in select mode.
		/// </summary>
		public void SelectMode() {
			isEditMode = false;
		}

		public void EditMode() {
			//TODO: Find which chain we clicked on to trigger edit mode
			isEditMode = true;
		}

		
		public GameObject AddPointToActive(Vector3 pos, bool isChainStart = false) {
			var point = Instantiate(linePointPrefab, new Vector3(pos.x, pos.y, 0f), Quaternion.identity, activeChain.transform);
			if (isChainStart)
				point.GetComponent<CurvedLinePoint>().MakeChainStart();
			else
				point.GetComponent<CurvedLinePoint>().MakeChainNode();

			return point;
		}


		private void Update() {
			
			if (!activated) return;

			if(Input.GetMouseButton(0)) {
				//We have already selected a pathbuilder note, do the initial angle flow
				if (isHovering || !EditorInput.isOverGrid) return;
				if (timeline.selectedNotes.Count == 1 && timeline.selectedNotes[0] == startClickNote && timeline.selectedNotes[0].data.behavior == TargetBehavior.NR_Pathbuilder) {
					var mousePosV3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					var mousePos = new Vector2(mousePosV3.x, mousePosV3.y);

					var vecFromCenter = (mousePos - startClickNote.data.position);
					if(vecFromCenter.sqrMagnitude > 0.5f) {
						var angle = Vector2.SignedAngle(vecFromCenter.normalized, new Vector2(0, 1));
						float snappedAngle;
						if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) {
							snappedAngle = Mathf.Floor((Math.Abs(angle) + 2.5f) / 5.0f) * 5.0f;
							//snappedAngle = angle;
						}
						else {
							snappedAngle = Mathf.Floor((Math.Abs(angle) + 22.5f) / 45.0f) * 45.0f;
							
						}
						if(Math.Sign(angle) < 0) {
							snappedAngle = 180 + (180 - snappedAngle);
						}

						startClickNote.data.pathBuilderData.initialAngle = snappedAngle;
						timeline.ReapplyScale();
					}
				}
			}
			else {
				startClickNote = null;
			}

			if (Input.GetMouseButtonDown(0)) {
				if (isHovering || !EditorInput.isOverGrid) return;
				if(startClickNote == null && iconUnderMouse != null && !iconUnderMouse.target.transient) {

					if(iconUnderMouse.data.behavior != TargetBehavior.NR_Pathbuilder) {
						NRActionConvertNoteToPathbuilder action = new NRActionConvertNoteToPathbuilder();
						action.data = iconUnderMouse.data;

						timeline.Tools.undoRedoManager.AddAction(action);
					}

					timeline.DeselectAllTargets();
					iconUnderMouse.TrySelect();

					startClickNote = iconUnderMouse.target;

					if(timeline.selectedNotes.Count == 1) {
						SetPathbuilderStateToSelectedNote();
					}
				}
			}
			
			

			iconsUnderMouse = null;

			if(timeline.selectedNotes.Count == 1) {
				chainBuilderWindowSelectedControls.SetActive(true);
				chainBuilderWindowUnselectedControls.SetActive(false);
			}
			else {
				chainBuilderWindowSelectedControls.SetActive(false);
				chainBuilderWindowUnselectedControls.SetActive(true);
			}
		}


		private void SetPathbuilderStateToSelectedNote() {
			angleIncrement.value = timeline.selectedNotes[0].data.pathBuilderData.angle;
			angleIncrementIncrement.value = timeline.selectedNotes[0].data.pathBuilderData.angleIncrement;
			stepDistance.value = timeline.selectedNotes[0].data.pathBuilderData.stepDistance;
			stepIncrement.value = timeline.selectedNotes[0].data.pathBuilderData.stepIncrement;

			var intervalStr = "1/" + timeline.selectedNotes[0].data.pathBuilderData.interval;
			for(int i = 0; i < pathBuilderInterval.elements.Count; ++i) {
				if(pathBuilderInterval.elements[i] == intervalStr) {
					pathBuilderInterval.defaultIndex = i;
					pathBuilderInterval.UpdateToIndex(i);
					break;
				}
			}
		}

		public void DrawTempChain() {
			List<Vector2> points = FindPointsAlongChain(10);
			if (points != null) DrawPointsAlongChain(points);
		}




		private void DrawPointsAlongChain(List<Vector2> points) {

			RemoveTempPointsAlongChain();

			foreach (Vector2 point in points) {
				Instantiate(nodeTempPointPrefab, new Vector3(point.x, point.y, 0f), Quaternion.identity, tempNodeIconsParent);
			}
		}

		private void RemoveTempPointsAlongChain() {
			foreach (Transform child in tempNodeIconsParent) {
				Destroy(child.gameObject);
			}
		}


		private List<Vector2> FindPointsAlongChain(int noteCount) {

			if (!activeChain) return null;

			//If we need to draw a new amount of notes or something else
			//if (noteCount == prevDrawPointsAmt) return null;

			prevDrawPointsAmt = noteCount;

			var lr = activeChain.GetComponent<LineRenderer>();
			int pCount = lr.positionCount;

			//If the amount of points on the line is lower than the positions we want to get, return to prevent crash.
			if (pCount < noteCount) return null;
			
			double indexDist = pCount / noteCount;


			List<Vector2> points = new List<Vector2>();

			for (double i = indexDist; i < pCount; i += indexDist) {
				//Get the previous index
				Vector3 pos1 = lr.GetPosition((int)Math.Floor(i));
				Vector3 pos2 = lr.GetPosition((int)Math.Ceiling(i));

				//4.3 -> 0.3
				double firstOffsetFromIndex = i - Math.Floor(i);
				//4.3 -> 0.7
				double secondOffsetFromIndex = Math.Ceiling(i) - i;

				if (firstOffsetFromIndex == 0) firstOffsetFromIndex = 1;

				Vector2 avg1 = pos1 * (float)firstOffsetFromIndex;
				Vector2 avg2 = pos2 * (float)secondOffsetFromIndex;

				Vector2 final = avg1 + avg2;
				points.Add(final);
			}

			var thing = points.Count;

			return points;
		}
	}
}