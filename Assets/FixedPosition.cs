using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedPosition : MonoBehaviour {

    public bool fixXPosition = false;
    public float xPosition = 0;
    public bool fixYPosition = false;
    public float yPosition = 0;
    public bool fixZPosition = false;
    public float zPosition = 0;
    
    void Awake () {
        Vector3 pos = transform.position;
        if (fixXPosition) pos.x = xPosition;
        if (fixYPosition) pos.y = yPosition;
        if (fixZPosition) pos.z = zPosition;
        transform.position = pos;
	}
}
