using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using NotReaper.UserInput;
using NotReaper.Grid;

public class TransformRotateTool : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] TransformTool transformTool;
    private bool isMouseDown = false;
    private Vector3 startMousePosition;
    private Vector3 startPosition;
    public bool shouldReturn;
    private Camera cam;
    public bool shouldSnap;
    float angle = 0f;

    private void Awake()
    {
        cam = Camera.main;
    }

    public void OnPointerDown(PointerEventData dt)
    {
        float angle = 0f;
        isMouseDown = true;
        startPosition = transform.position;
        startMousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        transformTool.SetPivotToCenterPoint();
        
    }

    public void OnPointerUp(PointerEventData dt)
    {

        isMouseDown = false;
        transformTool.RotateNotes(angle);
        transformTool.UpdateOverlay();

        if (shouldReturn)
        {
            transform.position = startPosition;
        }
    }

    void Update()
    {
        if (isMouseDown)
        {
            
            
            Vector3 transformToolStartPos = transformTool.transform.position;
            
            Vector3 mousePos = Input.mousePosition;
            Vector3 objectPos = Camera.main.WorldToScreenPoint(transformTool.transform.position);

            mousePos.x = mousePos.x - objectPos.x;
            mousePos.y = mousePos.y - objectPos.y;

            Vector3 currentPosition = mousePos;

            angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg - 90f;
            transformTool.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));


            //Vector3 diff = currentPosition - startMousePosition;

            //Vector3 pos = startPosition + diff;
            //pos.z = transform.position.z;

            //if (!shouldSnap)
            //{
            //    transform.position = pos;
            //}
            //else transform.position = NoteGridSnap.SnapToGrid(new Vector3(pos.x, pos.y, -1f), EditorInput.selectedSnappingMode);
        }
    }


}

