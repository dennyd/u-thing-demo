using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour {

	ArrayList selectedItems= new ArrayList();
	// Use this for initialization

	void Start () {

	}

	// Update is called once per frame
	void Update () {
		bool lclick = Input.GetMouseButtonUp (0), rclick = Input.GetMouseButtonUp (1);
		if (lclick || rclick) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {

				bool isUnit = hit.transform.gameObject.tag.Equals ("unit");
				if (lclick) {
					if (isUnit) {

						if (!(Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))) {
							foreach (GameObject obj in this.selectedItems) {
								deactivateUnit (obj);
							}
							selectedItems.Clear ();
						} 

						Debug.Log (selectedItems.IndexOf (hit.transform.gameObject) + " " + hit.transform.gameObject.name);
						if (selectedItems.IndexOf (hit.transform.gameObject) >= 0) {
							selectedItems.Remove (hit.transform.gameObject);
							deactivateUnit (hit.transform.gameObject);
						} else {
							selectedItems.Add (hit.transform.gameObject);
						}

					} else {
						clearSelection ();
					}
				} else if (rclick) {
					if (!isUnit && this.selectedItems.Count >= 0) {
						moveCommand (hit.point);
					}
				}
				foreach (GameObject obj in selectedItems) {
					activateUnit (obj);
				}
			}
		}
	}

	void deactivateUnit(GameObject u) {
		try {
			u.GetComponent<Animator> ().SetBool ("Selected", false);
		} catch {
			Debug.Log ("Could Not Deactivate Unit");
		}
	}

	void activateUnit(GameObject u) {
		try {
			u.GetComponent<Animator> ().SetBool("Selected", true);
		} catch {
			Debug.Log ("Could Not Activate Unit");
		}
	}

	void clearSelection() {
		foreach (GameObject obj in this.selectedItems) {
			obj.GetComponent<Animator> ().SetBool ("Selected", false);
		}
		this.selectedItems.Clear ();
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
		
}
