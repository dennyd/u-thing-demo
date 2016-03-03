﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class CircleColorScheme {
	public Color ownColor = new Color (0.5f, 0.5f, 1f, 1f);
	public Color enemyColor = new Color (1f, 0.4f, 0.2f, 1f);
	public Color selectedColor = new Color (1f, 1f, 0.5f, 1f);
	public Color neutralColor = new Color (0.5f, 0.5f, 0.5f, 1f);
};

public class UnitBehaviour : MonoBehaviour {

	public GameObject selectionCircle, selectionCirclePrefab;
	public bool Selected = false;
	public int id = 1;

	public CircleColorScheme colorScheme = new CircleColorScheme();
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
			Color selectionTintColor = colorScheme.ownColor;
		if (Selected) {
			selectionTintColor = colorScheme.selectedColor;
		}
		if (GetComponent<UnitBelonging> ().isNeutral ()) {
			selectionTintColor = colorScheme.neutralColor;
		} else if (!GetComponent<UnitBelonging> ().isMainPlayer()) {
			selectionTintColor = colorScheme.enemyColor;
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
