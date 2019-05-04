using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour {

    public Timeline timeline;
    Transform[] transforms;
    Vector3[] positions;
    public float speed = 1;
    public float scale = 1;
    public float magnitude = 1;

    public Transform prefab;

    public int rows;
    public int columns;
    public float radius = 10;    
    public float objectWidth = 1.155f;
    Transform[,] hexes;

    float rowSeparator;
    float anglePerObject;
    float oddRowOffset;

    // Use this for initialization
    void Awake () {

        rowSeparator = 0.433f * objectWidth;
        anglePerObject = ((objectWidth + rowSeparator) / (Mathf.PI * radius * 2)) * 360;
        oddRowOffset = anglePerObject / 2;
        hexes = new Transform[columns, rows];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                hexes[x,y] = Instantiate(prefab, transform);
                hexes[x,y].gameObject.layer = LayerMask.NameToLayer("Ignore main light");
            }
        }

        transform.Rotate(0, -anglePerObject * columns / 2f, 0);
    }
    
    Vector3 OnCircle(Vector3 c, float radius, float angle)
    { 
        Vector3 pos = new Vector3(); 
        pos.x = c.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        pos.z = c.z + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        pos.y = c.y; 
        return pos; 
    }

	void Update () {

        Vector3 center = Vector3.back * radius;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                float r = radius + magnitude * Mathf.PerlinNoise(Timeline.time * speed + x * scale, y * scale);
                var pos = OnCircle(center, r, x * anglePerObject + oddRowOffset * (y % 2));
                var rot = Quaternion.FromToRotation(Vector3.forward, pos - center);
                try
                {
                    hexes[x, y].localPosition = pos + Vector3.down * rowSeparator * rows / 2f;
                    hexes[x, y].position += Vector3.left * transform.localScale.x * radius * Mathf.Sin(Mathf.Deg2Rad * anglePerObject * columns / 2f);
                    hexes[x, y].localRotation = rot;
                } catch(Exception e)
                {
                    Debug.LogError(x + " " + y);
                }
            }
            center += Vector3.up * rowSeparator;
        }
	}
}
