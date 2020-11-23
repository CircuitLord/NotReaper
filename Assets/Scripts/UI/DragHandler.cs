using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using NotReaper.UserInput;
using NotReaper.Grid;

public class DragHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isMouseDown = false;
    private Vector3 startMousePosition;
    private Vector3 startPosition;
    public bool shouldReturn;
    private Camera cam;
    public bool shouldSnap;

    private void Awake()
    {
        cam = Camera.main;
    }

    public void OnPointerDown(PointerEventData dt)
    {
        isMouseDown = true;
        startPosition = transform.position;
        startMousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnPointerUp(PointerEventData dt)
    {

        isMouseDown = false;

        if (shouldReturn)
        {
            transform.position = startPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMouseDown)
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            Vector3 currentPosition = mousePos;

            Vector3 diff = currentPosition - startMousePosition;

            Vector3 pos = startPosition + diff;
            pos.z = transform.position.z;

            if (!shouldSnap)
            {
                transform.position = pos; 
            }
            else transform.position = NoteGridSnap.SnapToGrid(new Vector3(pos.x, pos.y, -1f), EditorInput.selectedSnappingMode);
        }
    }
}