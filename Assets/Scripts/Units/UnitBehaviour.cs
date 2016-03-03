using UnityEngine;
using System.Collections;

public class UnitBehaviour : MonoBehaviour {

	public GameObject selectionCircle, selectionCirclePrefab;
	public bool Selected = false;
	public int id = 1;
	// Use this for initialization
	void Start () {
		selectionCircle = Instantiate( selectionCirclePrefab );
		selectionCircle.transform.SetParent( transform, false );
		selectionCircle.transform.eulerAngles = new Vector3( 90, 0, 0 ); 

		selectionCircle.name = "Projector#" + id;

		Projector circleProjector = (Projector) selectionCircle.GetComponent<Projector> ();
		Material mat = circleProjector.material;
		circleProjector.material = new Material(mat);
	}
	
	// Update is called once per frame
	void Update () {
		Color selectionTintColor = new Color (0.7f, 0.7f, 0.7f, 0.2f);
		if (Selected) {
			selectionTintColor = new Color (1f, 1f, 0.5f, 0.5f);
		}
		selectionCircle.GetComponent<Projector> ().material.SetColor ("_Color", selectionTintColor);
	}

	public void SetSelect(bool selectStatus) {
		Selected = selectStatus;
	}

	public void SetCircleId(int id) {
		this.id = id;
		selectionCircle.name = "Projector#" + id;
	}
}
