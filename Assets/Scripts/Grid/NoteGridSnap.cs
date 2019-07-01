using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotReaper.Grid {


    public class NoteGridSnap : MonoBehaviour {

        [SerializeField] private float xSize;
        [SerializeField] private float ySize;
        private Vector2 gridOffset;

        private void Start() {
            gridOffset = transform.position;
        }


        public Vector2 GetNearestPointOnGrid(Vector2 pos) {

            pos -= gridOffset;

            int x = Mathf.RoundToInt(pos.x / xSize);
            int y = Mathf.RoundToInt(pos.y / ySize);

            Vector2 result = new Vector2((float) x * xSize, (float) y * ySize);

            result += gridOffset;

            return result;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            for (float i = 0; i < 40; i += xSize) {
                for (float y = 0; y < 40; y += ySize) {
                    var point = GetNearestPointOnGrid(new Vector2(i, y));
                }
            }
        }


    }


}