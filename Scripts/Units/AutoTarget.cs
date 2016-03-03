using UnityEngine;
using System.Collections;

public class AutoTarget : MonoBehaviour {

	Combat _combat;
	UnitBelonging _gameData;

	// Use this for initialization
	void Start () {
		_combat = GetComponent<Combat> ();
		_gameData = GetComponent<UnitBelonging> ();
	}
	
	// Update is called once per frame
	void Update () {
		Combat[] objs = FindObjectsOfType<Combat> ();
		float[] distances = new float[objs.Length];
		int i = 0;
		foreach (Combat cmb in objs) {
			UnitBelonging b = (UnitBelonging) cmb.GetComponent<UnitBelonging> ();
			if (!cmb.isAlive () || b.isSamePlayerAs(_gameData))
				distances [i] = Mathf.Infinity;
			else distances [i] = Vector3.Distance (transform.position, cmb.transform.position);
			i += 1;
		}
		float min = Mathf.Infinity;
		foreach (float dist in distances) {
			if (dist > 0 && min > dist)
				min = dist;
		}
		GameObject target = _combat.target;
		if (min > _combat.targetRange)
			target = null;
		else if (target == null || Vector3.Distance(transform.position, target.transform.position) > _combat.targetRange) target = objs [System.Array.IndexOf (distances, min)].gameObject;

		_combat.SetTarget (target);
	}
}
