using UnityEngine;
using System.Collections;

public class SimpleExpansion : MonoBehaviour {

	public int spawnInterval = 20;
	public MapPreset mapSettings;
	public GameObject unitPreset;
	// Use this for initialization

	private float lastSpawn;
	void Start () {
		lastSpawn = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (unitPreset != null && mapSettings != null) {
			if (Time.time - lastSpawn > spawnInterval) {
				StartCoroutine (clone ());

				lastSpawn = Time.time;
			}
		}
	}

	private IEnumerator clone() {

		GameObject clone = GetComponent<Cloneable> ().clone ();

		if (clone != null) {

			yield return new WaitForSeconds(0.5f);
			clone.transform.position = new Vector3 (
				Mathf.Clamp (clone.transform.position.x, 0, mapSettings.size),
				Mathf.Clamp (clone.transform.position.x, 0, mapSettings.height),
				Mathf.Clamp (clone.transform.position.x, 0, mapSettings.size)
			);

			Vector3 move = new Vector3 (Random.Range (0, mapSettings.size), mapSettings.height, Random.Range (0, mapSettings.size));
			Debug.Log ("Moving to " + move);
			clone.GetComponent<UnitMoveable> ().MoveTo (move);
		}
	}
}