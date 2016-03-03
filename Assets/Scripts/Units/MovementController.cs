using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour {

	ArrayList selectedItems = new ArrayList(), originalSelectedItems = new ArrayList();
	bool isSelecting = false;
	Vector3 mousePosition;
	// Use this for initialization

	void Start () {

	}

	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) mousePosition = Input.mousePosition;

		// Clicks
		bool lclick = Input.GetMouseButtonUp (0), rclick = Input.GetMouseButtonUp (1), shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		if (lclick || rclick) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {

				bool isUnit = hit.transform.gameObject.tag.Equals ("unit");
				if (lclick) {
					if (Vector3.Distance (mousePosition, Input.mousePosition) <= 1) {
						if (isUnit) {

							if (shift) {
								if (selectedItems.IndexOf (hit.transform.gameObject) >= 0) {
									deactivateUnit (hit.transform.gameObject);
								} else {
									activateUnit (hit.transform.gameObject);
								}
							} else {
								clearSelection ();
								activateUnit (hit.transform.gameObject);
							}
		
						} else {
							clearSelection ();
						}
					}
				} else if (rclick) {
					if (selectedItems.Count >= 0) {
						if (isUnit) {
							targetCommand (hit.transform.gameObject);
						} else {
							if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) {
								targetCommand (null);
							} else moveCommand (hit.point);
						}
					}
				}
			}
		}

		// Selection Boxes

		// If we press the left mouse button, save mouse location and begin selection
		if (Input.GetMouseButtonDown (0)) {
			originalSelectedItems = (ArrayList) selectedItems.Clone ();
			isSelecting = true;
		}

		// If we let go of the left mouse button, end selection
		if( Input.GetMouseButtonUp( 0 ) ) isSelecting = false;
		
		// Highlight all objects within the selection box
		if( isSelecting ) {
			if (Vector3.Distance (mousePosition, Input.mousePosition) >= 5) { 
				if (!shift) clearSelection ();
				foreach (UnitBehaviour unit in FindObjectsOfType<UnitBehaviour>()) {
					if (IsWithinSelectionBounds (unit.gameObject)) {
						try {
							if (shift && originalSelectedItems.Contains(unit.gameObject)) {
								deactivateUnit (unit.gameObject);
							} else activateUnit (unit.gameObject);
						} catch {
							Debug.Log ("Had a problem selecting units");
						}
					}
				}
			}
		}
	}

	public bool IsWithinSelectionBounds( GameObject gameObject ) {
		if( !isSelecting ) return false;

		var camera = Camera.main;
		var viewportBounds = Utils.GetViewportBounds( camera, mousePosition, Input.mousePosition );
		return viewportBounds.Contains( camera.WorldToViewportPoint( gameObject.transform.position ) );
	}


	void OnGUI()
	{
		if( isSelecting )
		{
			// Create a rect from both mouse positions
			var rect = Utils.GetScreenRect( mousePosition, Input.mousePosition );
			Utils.DrawScreenRect( rect, new Color( 0.8f, 0.8f, 0.95f, 0.25f ) );
			Utils.DrawScreenRectBorder( rect, 2, new Color( 0.8f, 0.8f, 0.95f ) );
		}
	}

	public void deactivateUnit(GameObject u, bool autoRemove = true) {
		try {
			u.GetComponent<UnitBehaviour> ().SetSelect(false);
			u.GetComponent<Animator> ().SetBool ("Selected", false);
		} catch {
			Debug.Log ("Could Not Deactivate Unit");
		}
		if (autoRemove && selectedItems.IndexOf(u) >= 0) selectedItems.Remove (u);
	}

	public void activateUnit(GameObject u, bool autoAdd = true) {
		try {
			u.GetComponent<UnitBehaviour> ().SetSelect(true);
			u.GetComponent<Animator> ().SetBool("Selected", true);
		} catch {
			Debug.Log ("Could Not Activate Unit");
		}
		if (autoAdd && selectedItems.IndexOf(u) < 0) selectedItems.Add (u);
	}

	void clearSelection() {
		foreach (GameObject obj in selectedItems) {
			deactivateUnit (obj, false);
		}
		selectedItems.Clear ();
	}

	void moveCommand(Vector3 point) {

		if (selectedItems.Count >= 0) {
			
			Vector3 avgDist = Vector3.zero;
			foreach (GameObject obj in selectedItems) {
				avgDist += obj.transform.position;
			}
			avgDist /= selectedItems.Count;

			foreach (GameObject obj in selectedItems) {
				try {
					UnitMoveable moveScript = obj.GetComponent<UnitMoveable>();
					if (moveScript) {
						moveScript.MoveTo (point + (obj.transform.position - avgDist));
					}
				} catch {
					Debug.Log ("Error");
				}
			}
		}
	}

	void targetCommand(GameObject target) {
		if (selectedItems.Count >= 0) {
			foreach (GameObject obj in selectedItems) {
				try {
					Combat combat = obj.GetComponent<Combat>();
					if (combat.isAlive()) combat.SetTarget(target);
				} catch {
					Debug.Log ("Could Not Set Target"); 
				}
			}
		}
	}
		
}
