using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class NoteGrid : MonoBehaviour {

    public enum SnappingMode { None, Grid, Melee };
    static SnappingMode snapMode = SnappingMode.Grid;
    bool hover = false;

    public Transform ghost;
    public LayerMask notesLayer;
    public GameObject standardGrid;
    public GameObject meleeGrid;

    [SerializeField] private Timeline timeline;


    private void Awake()
    {
        SetSnappingMode(SnappingMode.Grid);
    }

    private void Update()
	{
		if (hover)
		{
			if (EventSystem.current.IsPointerOverGameObject())
				return;

			var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos = SnapToGrid(mousePos);
			var ghostPos = ghost.position;
			ghostPos.x = mousePos.x;
			ghostPos.y = mousePos.y;
			ghost.position = ghostPos;

			if (Input.GetMouseButtonDown(0))
			{
				if (EventSystem.current.IsPointerOverGameObject())
					return;

				timeline.AddTarget(mousePos.x, mousePos.y);
			}
		}

		if (Input.GetMouseButtonDown(1))
		{
			if (EventSystem.current.IsPointerOverGameObject())
				return;

			timeline.DeleteTarget(NoteUnderMouse());
		}


        //TODO: Moved new hotkeys to the new area
		HandleKeybinds();

	}

	private void HandleKeybinds()
	{
		if (Input.GetKeyDown(KeyCode.G))
		{
			SetSnappingMode(SnappingMode.Grid);
		}
		if (Input.GetKeyDown(KeyCode.M))
		{
			SetSnappingMode(SnappingMode.Melee);
		}
		if (Input.GetKeyDown(KeyCode.N))
		{
			SetSnappingMode(SnappingMode.None);
		}

	}

	private Target NoteUnderMouse()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        ray.origin = new Vector3(ray.origin.x, ray.origin.y, -1f);
        ray.direction = Vector3.forward;
        Debug.DrawRay(ray.origin,ray.direction);
        if (Physics.Raycast(ray, out hit, 2, notesLayer))
        {
            Transform objectHit = hit.transform;
            Target target = objectHit.GetComponent<Target>().gridTarget;
            
            return target;
        }
        return null;
    }

    private void OnMouseEnter()
    {
        hover = true;
        ghost.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        hover = false;
        ghost.gameObject.SetActive(false);
    }

    public void SetSnappingMode(SnappingMode snappingMode)
    {
        snapMode = snappingMode;
        meleeGrid.SetActive(snapMode == SnappingMode.Melee);
        standardGrid.SetActive(snapMode == SnappingMode.Grid);
    }

    public static Vector3 SnapToGrid(Vector3 pos)
    {
        switch (snapMode)
        {
            case SnappingMode.Grid:
                return new Vector3(Mathf.Round(pos.x + 0.5f) - 0.5f, Mathf.Round(pos.y), pos.z);
            case SnappingMode.Melee:
                return new Vector3(Mathf.Sign(pos.x)*2, Mathf.Sign(pos.y), pos.z);
        }
        return pos;
    }
}
