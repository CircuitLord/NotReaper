using System.Collections;
using System.Collections.Generic;
using NotReaper.Targets;
using UnityEngine;
using UnityEngine.EventSystems;
using NotReaper.UserInput;

namespace NotReaper.Grid {


    public enum SnappingMode { None, Grid, Melee, DetailGrid }

    public class NoteGridSnap {

        private static Vector2 GetNearestPointOnGrid(Vector2 pos, SnappingMode mode) {

            //pos -= gridOffset; //Enable if grid is actually offset.
            pos.y += 0.45f;
            int x = Mathf.FloorToInt(pos.x / NotePosCalc.xSize);
            int y = Mathf.FloorToInt(pos.y / NotePosCalc.ySize);

            Vector2 result = new Vector2((float) x * NotePosCalc.xSize, (float) y * NotePosCalc.ySize);
            result.x += NotePosCalc.xSize / 2; //0.65f; //from 1.3 / 2

            //result += gridOffset; //Enable if grid is actually offset.

            return result;
        }

        public static Vector3 SnapToGrid(Vector3 pos, SnappingMode mode) {
            switch (mode) {
                case SnappingMode.Grid:
                case SnappingMode.DetailGrid:
                    return GetNearestPointOnGrid(pos, mode);
                case SnappingMode.Melee:
                    return new Vector3(Mathf.Sign(pos.x * NotePosCalc.xSize) * 2, Mathf.Sign(pos.y * NotePosCalc.ySize), pos.z + 5);
            }
            return new Vector3(pos.x, pos.y, pos.z + 5);
        }


    }


}