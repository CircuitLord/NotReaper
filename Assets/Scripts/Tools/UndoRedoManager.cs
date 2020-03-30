using System.Linq;
using System.Collections.Generic;
using NotReaper.Grid;
using NotReaper.Targets;
using NotReaper.UserInput;
using NotReaper.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using NotReaper.Managers;
using NotReaper.Timing;
using System;

namespace NotReaper.Tools {


	public class UndoRedoManager : MonoBehaviour {

		/// <summary>
		/// Contains the complete list of actions the user has done recently.
		/// </summary>
		public List<NRAction> actions = new List<NRAction>();

		/// <summary>
		/// Contains the actions the user has "undone" for future use.
		/// </summary>
		public List<NRAction> redoActions = new List<NRAction>();

		public int maxSavedActions = 20;

		public Timeline timeline;

		public void Undo() {
			if (actions.Count <= 0) return;

			NRAction action = actions.Last();
			Debug.Log("Undoing action:" + action.ToString());

			action.UndoAction(timeline);
			
			redoActions.Add(action);
			actions.RemoveAt(actions.Count - 1);

			
		}

		public void Redo() {

			if (redoActions.Count <= 0) return;

			NRAction action = redoActions.Last();

			Debug.Log("Redoing action:" + action.ToString());

			action.DoAction(timeline);

			actions.Add(action);
			redoActions.RemoveAt(redoActions.Count - 1);
		}

		public void AddAction(NRAction action) {
			action.DoAction(timeline);

			if (actions.Count <= maxSavedActions) {
				actions.Add(action);
			} else {
				while (maxSavedActions > actions.Count) {
					actions.RemoveAt(0);
				}

				actions.Add(action);
			}

			redoActions = new List<NRAction>();
		}

		public void ClearActions() {
			actions = new List<NRAction>();
			redoActions = new List<NRAction>();
		}
	}

	public abstract class NRAction {
		public abstract void DoAction(Timeline timeline);
		public abstract void UndoAction(Timeline timeline);
	}

	public class NRActionAddNote : NRAction {
		public TargetData targetData;

		public override void DoAction(Timeline timeline) {
			timeline.AddTargetFromAction(targetData);
		}
		public override void UndoAction(Timeline timeline) {
			timeline.DeleteTargetFromAction(targetData);
		}
	}

	public class NRActionMultiAddNote : NRAction {
		public List<TargetData> affectedTargets = new List<TargetData>();

		public override void DoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => { timeline.AddTargetFromAction(targetData); });
		}
		public override void UndoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => { timeline.DeleteTargetFromAction(targetData); });
		}
	}
	
	public class NRActionRemoveNote : NRAction {
		public TargetData targetData;

		public override void DoAction(Timeline timeline) {
			timeline.DeleteTargetFromAction(targetData);
		}
		public override void UndoAction(Timeline timeline) {
			timeline.AddTargetFromAction(targetData);
		}
	}
	
	public class NRActionMultiRemoveNote : NRAction {
		public List<TargetData> affectedTargets = new List<TargetData>();

		public override void DoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => { timeline.DeleteTargetFromAction(targetData); });
		}
		public override void UndoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => { timeline.AddTargetFromAction(targetData); });
		}
	}

	public class NRActionGridMoveNotes : NRAction {
		public List<TargetGridMoveIntent> targetGridMoveIntents = new List<TargetGridMoveIntent>();

		public override void DoAction(Timeline timeline) {
			targetGridMoveIntents.ForEach(intent => {
				intent.target.position = intent.intendedPosition;
			});
		}
		public override void UndoAction(Timeline timeline) {
			targetGridMoveIntents.ForEach(intent => {
				intent.target.position = intent.startingPosition;
			});
		}
	}

	public class NRActionTimelineMoveNotes : NRAction {
		public List<TargetTimelineMoveIntent> targetTimelineMoveIntents = new List<TargetTimelineMoveIntent>();

		public override void DoAction(Timeline timeline) {
			targetTimelineMoveIntents.ForEach(intent => {
				intent.target.time = intent.intendedTick;
			});
			timeline.SortOrderedList();
		}
		public override void UndoAction(Timeline timeline) {
			targetTimelineMoveIntents.ForEach(intent => {
				intent.target.time = intent.startTick;
			});
			timeline.SortOrderedList();
		}
	}

	public class NRActionSwapNoteColors : NRAction {
		public List<TargetData> affectedTargets = new List<TargetData>();

		public override void DoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => {
				switch (targetData.handType) {
					case TargetHandType.Left: 
						targetData.handType = TargetHandType.Right;
					break;
					
					case TargetHandType.Right: 
						targetData.handType = TargetHandType.Left;
					break;
				}

				if(targetData.behavior == TargetBehavior.NR_Pathbuilder) {
					switch (targetData.pathBuilderData.handType) {
						case TargetHandType.Left: 
							targetData.pathBuilderData.handType = TargetHandType.Right;
						break;
						
						case TargetHandType.Right: 
							targetData.pathBuilderData.handType = TargetHandType.Left;
						break;
					}

					targetData.handType = targetData.handType;
					ChainBuilder.ChainBuilder.GenerateChainNotes(targetData);
				}
			});
		}
		public override void UndoAction(Timeline timeline) {
			DoAction(timeline); //Swap is symmetrical
		}
	}

	public class NRActionHFlipNotes : NRAction {
		public List<TargetData> affectedTargets = new List<TargetData>();

		public float FlipAngle(float angle) {
			angle = ((angle + 180) % 360) - 180;
			return -angle;
		}

		public override void DoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => {
				targetData.x *= -1;

				if(targetData.behavior == TargetBehavior.NR_Pathbuilder) {
					targetData.pathBuilderData.initialAngle = FlipAngle(targetData.pathBuilderData.initialAngle);
					targetData.pathBuilderData.angle *= -1;
					targetData.pathBuilderData.angleIncrement *= -1;

					ChainBuilder.ChainBuilder.GenerateChainNotes(targetData);
				}
			});
		}
		public override void UndoAction(Timeline timeline) {
			DoAction(timeline); //Swap is symmetrical
		}
	}

	public class NRActionVFlipNotes : NRAction {
		public List<TargetData> affectedTargets = new List<TargetData>();

		public float FlipAngle(float angle) {
			angle = ((angle + 180) % 360) - 180;

			if (angle >= 0)
				return 180 - angle;
			else
				return -180 - angle;
		}

		public override void DoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => {
				targetData.y *= -1;

				if(targetData.behavior == TargetBehavior.NR_Pathbuilder) {
					targetData.pathBuilderData.initialAngle = FlipAngle(targetData.pathBuilderData.initialAngle);
					targetData.pathBuilderData.angle *= -1;
					targetData.pathBuilderData.angleIncrement *= -1;

					ChainBuilder.ChainBuilder.GenerateChainNotes(targetData);
				}
			});
		}
		public override void UndoAction(Timeline timeline) {
			DoAction(timeline); //Swap is symmetrical
		}
	}

	public class NRActionScale : NRAction {
		public List<TargetData> affectedTargets = new List<TargetData>();
		
		public float scale;

		public override void DoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => {
				if(targetData.behavior != TargetBehavior.Melee) {
					targetData.y *= scale;
					targetData.x *= scale;

					if(targetData.behavior == TargetBehavior.NR_Pathbuilder) {
						targetData.pathBuilderData.stepDistance *= scale;

						ChainBuilder.ChainBuilder.GenerateChainNotes(targetData);
					}
				}
			});
		}
		public override void UndoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => {
				if(targetData.behavior != TargetBehavior.Melee) {
					targetData.y /= scale;
					targetData.x /= scale;
					if(targetData.behavior == TargetBehavior.NR_Pathbuilder) {
						targetData.pathBuilderData.stepDistance /= scale;

						ChainBuilder.ChainBuilder.GenerateChainNotes(targetData);
					}
				}
			});	
		}
	}

	public class NRActionRotate : NRAction {
		public List<TargetData> affectedTargets = new List<TargetData>();

		public int rotateAngle = 0;

		public Vector2 rotateCenter = Vector2.zero;

		public void NRRotate(TargetData data, Vector2 center, int angle)
		{			
			data.x -= center.x;
			data.y -= center.y;
			angle = -angle;

			Vector2 rotate;

			rotate.x = (float)(data.x * Math.Cos(angle / 180f * Math.PI) + data.y * Math.Sin(angle / 180f * Math.PI));
			rotate.y = (float)(data.x * -Math.Sin(angle / 180f * Math.PI) + data.y * Math.Cos(angle / 180f * Math.PI));	
			rotate.x += center.x;
			rotate.y += center.y;

			data.x = rotate.x;
			data.y = rotate.y;
		}		
		public override void DoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => {
				if(targetData.behavior != TargetBehavior.Melee) {
					NRRotate(targetData, rotateCenter, rotateAngle);
					if(targetData.behavior == TargetBehavior.NR_Pathbuilder) {
						targetData.pathBuilderData.initialAngle -= rotateAngle;

						ChainBuilder.ChainBuilder.GenerateChainNotes(targetData);
					}
				}
			});
		}
		public override void UndoAction(Timeline timeline) {
			affectedTargets.ForEach(targetData => {
				if(targetData.behavior != TargetBehavior.Melee) {
					NRRotate(targetData, rotateCenter, -rotateAngle);
					if(targetData.behavior == TargetBehavior.NR_Pathbuilder) {
						targetData.pathBuilderData.initialAngle += rotateAngle;

						ChainBuilder.ChainBuilder.GenerateChainNotes(targetData);
					}
				}
			});	
		}
	}


	public class NRActionReverse : NRAction {
		public List<TargetData> affectedTargets = new List<TargetData>();
		public override void DoAction(Timeline timeline) {
            bool first = true;

            ulong firstTick = 0, lastTick = 0;
            
            //Find the first and last note in the sequence
            foreach (TargetData data in affectedTargets) {

                if (first) {
                    firstTick = data.time.tick;
                    lastTick = data.time.tick;
                    first = false;
                }
                
                else if (data.time.tick > lastTick) {
                    lastTick = data.time.tick;
                }

                else if (data.time.tick < firstTick) {
                    firstTick = data.time.tick;
                }
                
            }
            
            //Reverse the notes
            foreach (TargetData data in affectedTargets) {
                ulong amt = data.time.tick - firstTick;
                
                data.time = new QNT_Timestamp(lastTick - amt);
            }

			timeline.SortOrderedList();
		}
		public override void UndoAction(Timeline timeline) {
            bool first = true;

            ulong firstTick = 0, lastTick = 0;
            
            //Find the first and last note in the sequence
            foreach (TargetData data in affectedTargets) {

                if (first) {
                    firstTick = data.time.tick;
                    lastTick = data.time.tick;
                    first = false;
                }
                
                else if (data.time.tick > lastTick) {
                    lastTick = data.time.tick;
                }

                else if (data.time.tick < firstTick) {
                    firstTick = data.time.tick;
                }
                
            }
            
            //Reverse the notes
            foreach (TargetData data in affectedTargets) {
                ulong amt = data.time.tick - firstTick;
                
                data.time = new QNT_Timestamp(lastTick - amt);
            }
			
			timeline.SortOrderedList();
		}
	}

	public class NRActionSetTargetHitsound : NRAction {
		public List<TargetSetHitsoundIntent> targetSetHitsoundIntents = new List<TargetSetHitsoundIntent>();

		public override void DoAction(Timeline timeline) {
			targetSetHitsoundIntents.ForEach(intent => {
				intent.target.velocity = intent.newVelocity;
			});
		}
		public override void UndoAction(Timeline timeline) {
			targetSetHitsoundIntents.ForEach(intent => {
				intent.target.velocity = intent.startingVelocity;
			});
		}
	}

	public class NRActionSetTargetBehavior : NRAction {
		public List<TargetData> affectedTargets = new List<TargetData>();
		public TargetBehavior newBehavior;
		
		List<TargetBehavior> oldBehavior = new List<TargetBehavior>();
		List<TargetHandType> oldHandTypes = new List<TargetHandType>();
		List<TargetVelocity> oldVelocities = new List<TargetVelocity>();
		List<QNT_Duration> oldBeatLength = new List<QNT_Duration>();

		public override void DoAction(Timeline timeline) {
			oldBehavior = new List<TargetBehavior>();

			affectedTargets.ForEach(targetData => {
				TargetVelocity velocity = TargetVelocity.None;

				if(newBehavior == TargetBehavior.ChainStart) {
					velocity = TargetVelocity.ChainStart;
				}
				else if(newBehavior == TargetBehavior.Chain) {
					velocity = TargetVelocity.Chain;
				}
				else if(newBehavior == TargetBehavior.Melee) {
					velocity = TargetVelocity.Melee;
				}
				else if(newBehavior == TargetBehavior.Mine) {
					velocity = TargetVelocity.Mine;
				}
				else if(newBehavior == TargetBehavior.Standard ||
						newBehavior == TargetBehavior.Hold ||
						newBehavior == TargetBehavior.Horizontal ||
						newBehavior == TargetBehavior.Vertical) {
					velocity = TargetVelocity.Standard;
				}

				//Path notes and regular notes both use the same beat length
				oldBeatLength.Add(targetData.beatLength);

				if(targetData.behavior == TargetBehavior.NR_Pathbuilder) {
					oldBehavior.Add(targetData.pathBuilderData.behavior);
					oldHandTypes.Add(targetData.pathBuilderData.handType);
					oldVelocities.Add(targetData.pathBuilderData.velocity);
					
					targetData.pathBuilderData.behavior = newBehavior;
					if(velocity != TargetVelocity.None) targetData.pathBuilderData.velocity = velocity;

					//Fix hand type when going to melee
					if(newBehavior == TargetBehavior.Melee) {
						targetData.pathBuilderData.handType = TargetHandType.Either;
					}

					//Fixup hand type when coming from melee
					if(oldBehavior.Last() == TargetBehavior.Melee) {
						targetData.pathBuilderData.handType = targetData.handType;
					}

					ChainBuilder.ChainBuilder.GenerateChainNotes(targetData);
				}
				else {
					oldBehavior.Add(targetData.behavior);
					oldHandTypes.Add(targetData.handType);
					oldVelocities.Add(targetData.velocity);

					targetData.behavior = newBehavior;
					if(velocity != TargetVelocity.None) targetData.velocity = velocity;

					//Fix hand type when going to melee
					if(newBehavior == TargetBehavior.Melee) {
						targetData.handType = TargetHandType.Either;
					}

					//Fixup hand type when coming from melee
					if(oldBehavior.Last() == TargetBehavior.Melee) {
						targetData.handType = TargetHandType.Left;
					}
					
					if(TargetData.BehaviorSupportsBeatLength(newBehavior) && targetData.beatLength < Constants.QuarterNoteDuration) {
						targetData.beatLength = Constants.QuarterNoteDuration;
					}
				}
			});
		}
		public override void UndoAction(Timeline timeline) {
			for(int i = 0; i < affectedTargets.Count; ++i) {
				if(affectedTargets[i].behavior == TargetBehavior.NR_Pathbuilder) {
					affectedTargets[i].pathBuilderData.behavior = oldBehavior[i];
					affectedTargets[i].pathBuilderData.velocity = oldVelocities[i];
					affectedTargets[i].pathBuilderData.handType = oldHandTypes[i];
				}
				else {
					affectedTargets[i].behavior = oldBehavior[i];
					affectedTargets[i].handType = oldHandTypes[i];
					affectedTargets[i].velocity = oldVelocities[i];
				}

				affectedTargets[i].beatLength = oldBeatLength[i];
			}
		}
	}

	public class NRActionConvertNoteToPathbuilder : NRAction {
		public TargetData data;
		public QNT_Duration oldBeatLength;
		public PathBuilderData pathBuilderData = new PathBuilderData();

		public override void DoAction(Timeline timeline) {
			pathBuilderData.behavior = data.behavior;
			pathBuilderData.velocity = data.velocity;
			pathBuilderData.handType = data.handType;
			data.pathBuilderData = pathBuilderData;

			//Ensure the path builder always starts with a quarter note of build time
			oldBeatLength = data.beatLength;
			if(data.beatLength < Constants.QuarterNoteDuration) {
				data.beatLength = Constants.QuarterNoteDuration;
			}

			data.behavior = TargetBehavior.NR_Pathbuilder;
			ChainBuilder.ChainBuilder.GenerateChainNotes(data);
		}
		public override void UndoAction(Timeline timeline) {
			data.pathBuilderData.DeleteCreatedNotes(timeline);

			data.behavior = data.pathBuilderData.behavior;
			data.velocity = data.pathBuilderData.velocity;
			data.handType = data.pathBuilderData.handType;
			data.beatLength = oldBeatLength;
			data.pathBuilderData = null;
		}
	}

	public class NRActionBakePath : NRAction {
		public TargetData data;

		public override void DoAction(Timeline timeline) {
			//Generate and create real notes
			ChainBuilder.ChainBuilder.GenerateChainNotes(data);
			foreach(TargetData genData in data.pathBuilderData.generatedNotes) {
				timeline.AddTargetFromAction(new TargetData(genData));
			}

			//Destroy the path builder note (and all the generated transient notes)
			timeline.DeleteTargetFromAction(data);
		}
		public override void UndoAction(Timeline timeline) {
			timeline.AddTargetFromAction(data);

			//Recalculate the notes, and remove the "real" notes
			ChainBuilder.ChainBuilder.CalculateChainNotes(data);
			foreach(TargetData genData in data.pathBuilderData.generatedNotes) {
				var foundData = timeline.FindTargetData(genData.time, genData.behavior, genData.handType);
				if(foundData != null) {
					timeline.DeleteTargetFromAction(foundData);
				}
			}
			ChainBuilder.ChainBuilder.GenerateChainNotes(data);
		}
	}
}