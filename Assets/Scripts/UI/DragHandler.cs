using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
	public Transform target;
	private bool isMouseDown = false;
	private Vector3 startMousePosition;
	private Vector3 startPosition;
	public bool shouldReturn;

	// Use this for initialization
	void Start () {
	
	}

	public void OnPointerDown(PointerEventData dt) {
		isMouseDown = true;

		Debug.Log ("Draggable Mouse Down");

		startPosition = target.position;
		startMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	public void OnPointerUp(PointerEventData dt) {
		Debug.Log ("Draggable mouse up");

		isMouseDown = false;

		if (shouldReturn) {
			target.position = startPosition;
		}
	}

	// Update is called once per frame
	void Update () {
		if (isMouseDown) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			Vector3 currentPosition = mousePos;

			Vector3 diff = currentPosition - startMousePosition;

			Vector3 pos = startPosition + diff;
            pos.z = target.position.z;

			target.position = pos;
		}
	}
}