using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using NotReaper.UserInput;
using NotReaper.Grid;

public class TransformRotateTool : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] TransformTool transformTool;
    
    bool isMouseDown = false;
    Vector3 startMousePosition;
    Vector3 iconStartPosition;
    Camera cam;
    float angle = 0f;

    private void Awake()
    {
        cam = Camera.main;
    }

    public void OnPointerDown(PointerEventData dt)
    {
        float angle = 0f;
        isMouseDown = true;
        iconStartPosition = cam.WorldToScreenPoint(transform.position);
        startMousePosition = Input.mousePosition;
        transformTool.SetPivotToCenterPoint();
    }

    public void OnPointerUp(PointerEventData dt)
    {
        isMouseDown = false;
        transformTool.RotateNotes(angle);
        transformTool.UpdateOverlay();
    }

    void Update()
    {
        if (isMouseDown)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 objectPos = cam.WorldToScreenPoint(transformTool.transform.position);

            angle = (Mathf.Atan2(mousePos.y - objectPos.y, mousePos.x - objectPos.x) - Mathf.Atan2(iconStartPosition.y - objectPos.y, iconStartPosition.x - objectPos.x)) * Mathf.Rad2Deg;
            transformTool.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    float GetOffsetFromIcon(Vector2 icon, Vector2 point)
    {
        Vector2 dist = icon - point;
        return Mathf.Acos(dist.y / dist.magnitude) * Mathf.Rad2Deg; //Angle in radians
    }

}

