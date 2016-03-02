using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour {

	GameObject[] selectedItems;
	// Use this for initialization

	void Start () {

	}

	// Update is called once per frame
	void FixedUpdate () {
		bool lclick = Input.GetMouseButton (0), rclick = Input.GetMouseButton (1);
		if (lclick || rclick) {
			Debug.Log ("Click");
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
				Debug.Log (hit.transform.gameObject.tag);
			}
		}

	}
}
