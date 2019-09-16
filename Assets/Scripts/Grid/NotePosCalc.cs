using System;
using NotReaper.Models;
using NotReaper.Targets;
using UnityEngine;

namespace NotReaper.Grid {


	public static class NotePosCalc {

		public static float xSize = 1.1f;
		public static float ySize = 0.9f;
		public static float xStart = 6.05f;
		public static float yStart = 2.7f;

		public static readonly int CUE_NOTE_MIN_PITCH = 0;
		public static readonly int CUE_NOTE_MAX_PITCH = 83;
		public static readonly int CUE_MELEE_MIN_PITCH = 98;
		public static readonly int CUE_MELEE_MAX_PITCH = 101;

		//Gets the cue status based on a target.
		public static Cue ToCue(Target target, int offset, bool includeGridOffset) {

			int pitch = 0;
			Vector2 tempPos = new Vector2();
			int x = 0, y = 0;
			float offsetX, offsetY = 0;

			//If it's a melee note.
			if (target.behavior == TargetBehavior.Melee) {
				pitch = CUE_MELEE_MIN_PITCH;
				if (target.gridTargetIcon.transform.position.x > 0) pitch += 1;
				if (target.gridTargetIcon.transform.position.y > 0) pitch += 2;

				offsetX = 0;
				offsetY = 0;

			} else {

				//We have to divide by the new positions between the grid.
				tempPos.x = target.gridTargetIcon.transform.position.x / xSize;
				tempPos.y = target.gridTargetIcon.transform.position.y / ySize;

				//Offset it to all be positive.
				x = Mathf.RoundToInt(tempPos.x + xStart);
				y = Mathf.RoundToInt(tempPos.y + yStart);

				pitch = x + 12 * y;

				//Used to be 5.5f
				offsetX = (tempPos.x + xStart - x);

				offsetY = (tempPos.y + yStart - y);

				offsetX += 0.4499f;
				offsetY += 0.3f;


				// out of bounds check (some maps made outside NR use offsets to get around pitch min and max)
				if (pitch > CUE_NOTE_MAX_PITCH) {
					var difference = pitch - CUE_NOTE_MAX_PITCH;
					int rows = (int)(Math.Ceiling(difference / 12f));

					offsetY += rows;
					pitch -= rows * 12;

				} else if (pitch < CUE_NOTE_MIN_PITCH) {
					var difference = CUE_NOTE_MIN_PITCH - pitch;
					int rows = (int)(Math.Ceiling(difference / 12f));

					offsetY -= rows;
					pitch += rows * 12;
				}

			}

			Cue cue = new Cue() {
				tick = Mathf.RoundToInt(target.gridTargetIcon.transform.localPosition.z * 480f) + offset,
					tickLength = Mathf.RoundToInt(target.beatLength),//Mathf.RoundToInt(target.beatLength * 480f),
					pitch = pitch,
					velocity = target.velocity,
					gridOffset = new Cue.GridOffset { x = (float) Math.Round(offsetX, 2), y = (float) Math.Round(offsetY, 2) },
					handType = target.handType,
					behavior = target.behavior
			};


			return cue;
		}


		public static Vector2 PitchToPos(Cue cue) {

			float x = 0, y = 0;

			if (cue.behavior == TargetBehavior.Melee) {
				switch (cue.pitch) {
					case 98:
						x = -2f * xSize;
						y = -1 * ySize;
						break;
					case 99:
						x = 2f * xSize;
						y = -1 * ySize;
						break;
					case 100:
						x = -2f * xSize;
						y = 1 * ySize;
						break;
					case 101:
						x = 2f * xSize;
						y = 1 * ySize;
						break;

				}
			} else {			
				//x = (cue.pitch % 12) + (float) (cue.gridOffset.x * 1f) - 6.1f;
				//x = x * xSize;
				//y = cue.pitch / 12 + (float) (cue.gridOffset.y * 1f) - 2.7f;
				//y = y * ySize;

				//x -= 0.3f;
				//y -= 0.7f;

				int col = cue.pitch % 12;
                int row = cue.pitch / 12;
                x = -xStart + (col + (float)cue.gridOffset.x) * xSize;
                y = -yStart + (row + (float)cue.gridOffset.y) * ySize;
			}


			return new Vector2(x, y);
		}


	}
}