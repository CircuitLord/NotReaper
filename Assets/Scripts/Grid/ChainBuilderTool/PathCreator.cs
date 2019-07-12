using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotReaper.Grid.ChainBuilderTool {


    public class PathCreator : MonoBehaviour {

        [HideInInspector]
        public Path path;

        public void CreatePath() {
            path = new Path(transform.position);
        }
    }
}