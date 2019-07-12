using System;
using NotReaper.Models;
using NotReaper.Targets;
using UnityEngine;

namespace NotReaper.Grid {


	public static class NotePosCalc {

		private static float xSize = 1.3f;
		private static float ySize = 0.9f;


		//Gets the cue status based on a grid target.
		//TODO: New grid target
		public static Cue ToCue(GridTarget target, int offset, bool includeGridOffset) {

			int pitch = 0;
			Vector2 tempPos = new Vector2();
			int x = 0, y = 0;
			float offsetX, offsetY = 0;

			//If it's a melee note.
			if (target.behavior == TargetBehavior.Melee) {
				pitch = 98;
				if (target.transform.position.x > 0) pitch += 1;
				if (target.transform.position.y > 0) pitch += 2;

				offsetX = 0;
				offsetY = 0;

			} else {

				//We have to divide by the new positions between the grid.
				tempPos.x = target.transform.position.x / xSize;
				tempPos.y = target.transform.position.y / ySize;

				//Offset it to all be positive.
				x = Mathf.RoundToInt(tempPos.x + 5.5f);
				y = Mathf.RoundToInt(tempPos.y + 3);

				pitch = x + 12 * y;

				offsetX = (tempPos.x + 5.5f - x);
				offsetY = (tempPos.y + 3 - y);

			}

			Cue cue = new Cue() {
				tick = Mathf.RoundToInt(target.transform.localPosition.z * 480f) + offset,
					tickLength = Mathf.RoundToInt(target.beatLength * 480f),
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
				x = (cue.pitch % 12) + (float) (cue.gridOffset.x * xSize) - 5.5f;
				x = x * xSize;
				y = cue.pitch / 12 + (float) (cue.gridOffset.y * ySize) - 3f;
				y = y * ySize;
			}


			return new Vector2(x, y);
		}


	}
}