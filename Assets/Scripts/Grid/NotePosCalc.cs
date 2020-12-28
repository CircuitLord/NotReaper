using System;
using NotReaper.Models;
using NotReaper.Targets;
using UnityEngine;
using NotReaper.Timing;
namespace NotReaper.Grid {


	public static class NotePosCalc {

		public static float xSize = 1.1f;
		public static float ySize = 0.9f;
		public static float xStart = 6.05f;
		public static float yStart = 2.7f;


		//Gets the cue status based on a target.
		public static Cue ToCue(Target target, Relative_QNT offset) {

			int pitch = 0;
			Vector2 tempPos = new Vector2();
			int x = 0, y = 0;
			float offsetX = 0, offsetY = 0;
			
			Vector2 pitch98 = new Vector2(NotePosCalc.xSize * -2f, -NotePosCalc.ySize);
			Vector2 pitch99 = new Vector2(NotePosCalc.xSize * 2f, -NotePosCalc.ySize);
			Vector2 pitch100 = new Vector2(NotePosCalc.xSize * -2f, NotePosCalc.ySize);
			Vector2 pitch101 = new Vector2(NotePosCalc.xSize * 2f, NotePosCalc.ySize);

			//If it's a melee note.
			if (target.data.behavior == TargetBehavior.Melee || target.data.behavior == TargetBehavior.Mine) {
				pitch = 98;
				if (target.data.x > 0) pitch += 1;
				if (target.data.y > 0) pitch += 2;

				switch (pitch) {
					case 98:
						offsetX = pitch98.x - target.data.position.x;
						offsetY = pitch98.y - target.data.position.y;
						break;
					
					case 99:
						offsetX = pitch99.x - target.data.position.x;
						offsetY = pitch99.y - target.data.position.y;
						break;
					
					case 100:
						offsetX = pitch100.x - target.data.position.x;
						offsetY = pitch100.y - target.data.position.y;
						break;
					
					case 101:
						offsetX = pitch101.x - target.data.position.x;
						offsetY = pitch101.y - target.data.position.y;
						break;
						


				}

				offsetX = -offsetX / NotePosCalc.xSize / 4;
				offsetY = -offsetY / NotePosCalc.ySize / 4;
				

			} else {

				//We have to divide by the new positions between the grid.
				tempPos.x = (target.data.x  + xStart) / xSize;
				tempPos.y = (target.data.y + yStart) / ySize;

				x = Mathf.Clamp(Mathf.RoundToInt(tempPos.x), 0, 11);
            	y = Mathf.Clamp(Mathf.RoundToInt(tempPos.y), 0, 6);
            	pitch = x + 12 * y;

				offsetX = (tempPos.x - x);
				offsetY = (tempPos.y - y);			

			}

			Cue cue = new Cue() {
				tick = (int)(target.data.time + offset).tick,
				tickLength = (int)target.data.beatLength.tick,
				pitch = pitch,
				velocity = target.data.velocity,
				gridOffset = new Cue.GridOffset { x = (float) Math.Round(offsetX, 2), y = (float) Math.Round(offsetY, 2) },
				handType = target.data.handType,
				behavior = target.data.behavior,
			};

			if (NRSettings.config.useAutoZOffsetWith360) {
                cue.zOffset = GetZOffsetForX(target.data.x);
			}


			return cue;
		}


		public static Vector2 PitchToPos(Cue cue) {

			float x = 0, y = 0;

			if (cue.behavior == TargetBehavior.Melee || cue.behavior == TargetBehavior.Mine) {
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

				x += (float) cue.gridOffset.x * NotePosCalc.xSize * 4;
				y += (float) cue.gridOffset.y * NotePosCalc.ySize * 4;


			} else {			
				int col = cue.pitch % 12;
                int row = cue.pitch / 12;
                x = -xStart + (col + (float)cue.gridOffset.x) * xSize;
                y = -yStart + (row + (float)cue.gridOffset.y) * ySize;
			}


			return new Vector2(x, y);
		}



		private static float GetZOffsetForX(float x) {
			if (x < 0f) x *= -1f;

			if (x < 5.5f) return 0f;
			
			var zOffset = Mathf.Clamp(Mathf.Abs(x) - 5.5f, 0f, 2.5f) / 5f;

			return zOffset;
		}
		

	}
}