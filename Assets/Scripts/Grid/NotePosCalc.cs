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


		//Gets the cue status based on a target.
		public static Cue ToCue(Target target, int offset, bool includeGridOffset) {

			int pitch = 0;
			Vector2 tempPos = new Vector2();
			int x = 0, y = 0;
			float offsetX = 0, offsetY = 0;
			
			Vector2 pitch98 = new Vector2(NotePosCalc.xSize * -2f, -NotePosCalc.ySize);
			Vector2 pitch99 = new Vector2(NotePosCalc.xSize * 2f, -NotePosCalc.ySize);
			Vector2 pitch100 = new Vector2(NotePosCalc.xSize * -2f, NotePosCalc.ySize);
			Vector2 pitch101 = new Vector2(NotePosCalc.xSize * 2f, NotePosCalc.ySize);

			//If it's a melee note.
			if (target.data.behavior == TargetBehavior.Melee) {
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
				
				

				Debug.Log(offsetX);

				


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
				tick = Mathf.RoundToInt(target.data.beatTime * 480f) + offset,
					tickLength = Mathf.RoundToInt(target.data.beatLength),//Mathf.RoundToInt(target.beatLength * 480f),
					pitch = pitch,
					velocity = target.data.velocity,
					gridOffset = new Cue.GridOffset { x = (float) Math.Round(offsetX, 2), y = (float) Math.Round(offsetY, 2) },
					handType = target.data.handType,
					behavior = target.data.behavior
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


	}
}