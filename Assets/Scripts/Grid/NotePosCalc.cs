using System;
using NotReaper.Models;
using NotReaper.Targets;
using UnityEngine;

namespace NotReaper.Grid {


	public static class NotePosCalc {

		public static float xSize = 1.1f;
		public static float ySize = 0.9f;


		//Gets the cue status based on a target.
		public static Cue ToCue(Target target, int offset, bool includeGridOffset) {

			int pitch = 0;
			Vector2 tempPos = new Vector2();
			int x = 0, y = 0;
			float offsetX, offsetY = 0;

			//If it's a melee note.
			if (target.behavior == TargetBehavior.Melee) {
				pitch = 98;
				if (target.gridTargetIcon.transform.position.x > 0) pitch += 1;
				if (target.gridTargetIcon.transform.position.y > 0) pitch += 2;

				offsetX = 0;
				offsetY = 0;

			} else {

				//We have to divide by the new positions between the grid.
				tempPos.x = target.gridTargetIcon.transform.position.x / xSize;
				tempPos.y = target.gridTargetIcon.transform.position.y / ySize;

				//Offset it to all be positive.
				x = Mathf.RoundToInt(tempPos.x + 6.1f);
				y = Mathf.RoundToInt(tempPos.y + 2.7f);

				pitch = x + 12 * y;

				//Used to be 5.5f
				offsetX = (tempPos.x + 6.1f - x);

				offsetY = (tempPos.y + 2.7f - y);

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
				x = (cue.pitch % 12) + (float) (cue.gridOffset.x * xSize) - 6.1f;
				x = x * xSize;
				y = cue.pitch / 12 + (float) (cue.gridOffset.y * ySize) - 2.7f;
				y = y * ySize;
			}


			return new Vector2(x, y);
		}


	}
}