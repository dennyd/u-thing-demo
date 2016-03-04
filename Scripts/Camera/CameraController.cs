using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private Camera _camera;

	// Use this for initialization
	void Start () {
		_camera = GetComponent<Camera> ();
	}

	public float ZoomSpeed = 10f;

	public float RotateSpeed = 10f;

	public float DragSpeed = 20f;

	public float ScrollArea = 25;
	public float ScrollSpeed = 10f;

	public float LevelArea = 100;
	public float ZoomMin = 2;
	public float ZoomMax = 15;

	public int PanButton = 0;
	public int RotateButton = 2;

	// Update is called once per frame
	void Update () {
		if(enabled){
			doTransforms ();
			doRotates ();
		}	
	}

	private Vector3 mouseOrigin;
	void doTransforms() {

		var translation = Vector3.zero;

		// Zoom in or out
		var zoomDelta = Input.GetAxis("Mouse ScrollWheel")*ZoomSpeed*Time.deltaTime;
		if (zoomDelta != 0)
		{
			translation -= _camera.transform.forward * ZoomSpeed * zoomDelta;
		}

		// Keyboard move
		translation += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		// Mouse move
		if (Input.GetMouseButton (PanButton)) { // Right Mouse Button
			if (Input.GetMouseButtonDown (PanButton)) {
				mouseOrigin = Input.mousePosition;
			}
			Vector3 pos = _camera.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

			// Hold button and drag camera around
			translation -= _offset(_camera.transform.right, DragSpeed * Time.deltaTime * pos.x);
			translation -= _offset(_camera.transform.up, DragSpeed * Time.deltaTime * pos.y);
		} else {
			// Move camera if mouse pointer reaches screen borders
			if (Input.mousePosition.x >= 0 && Input.mousePosition.x < ScrollArea)
			{
				translation += _offset(_camera.transform.right, -ScrollSpeed * Time.deltaTime);
			}

			if (Input.mousePosition.x <= Screen.width && Input.mousePosition.x >= Screen.width - ScrollArea)
			{
				translation += _offset(_camera.transform.right, ScrollSpeed * Time.deltaTime);
			}

			if (Input.mousePosition.y >= 0 && Input.mousePosition.y < ScrollArea)
			{
				translation += _offset(_camera.transform.up, -ScrollSpeed * Time.deltaTime);
			}

			if (Input.mousePosition.y <= Screen.height && Input.mousePosition.y > Screen.height - ScrollArea)
			{
				translation += _offset(_camera.transform.up, ScrollSpeed * Time.deltaTime);
			}
		}

		var desiredPosition = _camera.transform.position + translation;
		if (desiredPosition.x < -LevelArea || LevelArea < desiredPosition.x)
		{
			translation.x = 0;
		}
		if (desiredPosition.y < ZoomMin || ZoomMax < desiredPosition.y)
		{
			translation = Vector3.zero;
		}
		if (desiredPosition.z < -LevelArea || LevelArea < desiredPosition.z)
		{
			translation.z = 0;
		}

		_camera.transform.position += translation;
	}

	Vector3 _offset(Vector3 vect, float offset) {
		return new Vector3 (vect.x * offset, 0, vect.z * offset);
	}

	void doRotates() {


		if (Input.GetMouseButton (RotateButton)) {
			if (Input.GetMouseButtonDown (RotateButton)) {
				mouseOrigin = Input.mousePosition;
			}
			
			Vector3 pos = _camera.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

			_camera.transform.RotateAround(transform.position, transform.right, -pos.y * RotateSpeed);
			_camera.transform.RotateAround(transform.position, Vector3.up, pos.x * RotateSpeed);
		}

	}
}
