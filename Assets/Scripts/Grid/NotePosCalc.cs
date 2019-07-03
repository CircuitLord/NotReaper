using System;
using NotReaper.Models;
using NotReaper.Targets;
using UnityEngine;

namespace NotReaper.Grid {


	public static class NotePosCalc {

		private static float xSize = 1.3f;
		private static float ySize = 0.9f;


		//Gets the cue status based on a grid target.
		public static Cue ToCue(GridTarget gridTarget, int offset) {

			int pitch = 0;
			Vector2 tempPos = new Vector2();
			int x = 0, y = 0;
			float offsetX, offsetY = 0;

			//If it's a melee note.
			if (gridTarget.behavior == TargetBehavior.Melee) {
				pitch = 98;
				if (gridTarget.transform.position.x > 0) pitch += 1;
				if (gridTarget.transform.position.y > 0) pitch += 2;

				offsetX = 0;
				offsetY = 0;

			} else {

				//We have to divide by the new positions between the grid.
				tempPos.x = tempPos.x / xSize;
				tempPos.y = tempPos.y / ySize;

				//Offset it to all be positive.
				x = Mathf.RoundToInt(tempPos.x + 5.5f);
				y = Mathf.RoundToInt(tempPos.y + 3);

				pitch = x + 12 * y;

				offsetX = gridTarget.transform.position.x + 5.5f - x;
				offsetY = gridTarget.transform.position.y + 3 - y;

			}

			Cue cue = new Cue() {
				tick = Mathf.RoundToInt(gridTarget.transform.localPosition.z * 480f) + offset,
					tickLength = Mathf.RoundToInt(gridTarget.beatLength * 480f),
					pitch = pitch,
					velocity = gridTarget.velocity,
					gridOffset = new Cue.GridOffset { x = (float) Math.Round(offsetX, 2), y = (float) Math.Round(offsetY, 2) },
					handType = gridTarget.handType,
					behavior = gridTarget.behavior
			};


			return cue;
		}

		//public static Vector2 GetOffset(Cue cue) {

		//}

		public static Vector2 PitchToPos(Cue cue) {

			float x = 0, y = 0;

			if (cue.behavior == TargetBehavior.Melee) {
				switch (cue.pitch) {
					case 98:
						x = -2f;
						y = -1;
						break;
					case 99:
						x = 2f;
						y = -1;
						break;
					case 100:
						x = -2f;
						y = 1;
						break;
					case 101:
						x = 2f;
						y = 1;
						break;

				}
			} else {
				x = (cue.pitch % 12) + (float) cue.gridOffset.x - 5.5f;
				y = cue.pitch / 12 + (float) cue.gridOffset.y - 3f;
			}


			return new Vector2();
		}


	}
}