using UnityEngine;
using System.Collections;
	
[System.Serializable]
public class ElevationToColor {
	public Color one = new Color (0.9f, 0.9f, 0.9f, 1f);
	public Color two = new Color (0.4f, 0.4f, 0.2f, 1f);
	public Color three = new Color (0.4f, 0.7f, 0.3f, 1f);
	public Color four = new Color (0.2f, 0.4f, 0.9f, 1f);
	public Color def = new Color(0.5f, 0.5f, 0.5f, 1f);

	public Color getColor(string s) {
		return (Color) GetType ().GetField (s).GetValue (this);
	}
	public float getColorVariant(string s, string v) {
		Color c = getColor (s);
		return (float) c.GetType ().GetField (v).GetValue (c);
	}
}

public class Cloneable : MonoBehaviour {

	private UnitBehaviour behaviour;
	public Color baseColor = Color.black, currentColor = Color.black;
	public ElevationToColor colorScheme = new ElevationToColor();
	public GameObject clonePrefab;
	// Use this for initialization
	void Start () {
		if (baseColor.Equals(Color.black)) baseColor = colorScheme.def;
		if (currentColor.Equals(Color.black)) setColor(baseColor);
		behaviour = GetComponent<UnitBehaviour> ();
	}

	// Update is called once per frame
	void Update () {
		foreach (Renderer rend in transform.GetComponentsInChildren<Renderer>()) {
			rend.material.color = currentColor;
		}
	}

	void OnMouseOver() {
		if (behaviour.Selected) {
			if (Input.GetMouseButtonDown (2)) {
				GameObject instance = (GameObject) Instantiate (clonePrefab, transform.position + new Vector3(Mathf.Sign(Random.Range(-1, 1)) * Random.Range(1, 5), 10, Mathf.Sign(Random.Range(-1, 1)) * Random.Range(1, 5)), transform.rotation);
				float elevation = getElevation ();
				string tex;
				if (elevation >= 0.8)
					tex = "one";
				else if (elevation >= 0.5)
					tex = "two";
				else if (elevation >= 0.2)
					tex = "three";
				else
					tex = "four";
				Color mixBaseColor = currentColor;
				if (mixBaseColor.Equals(colorScheme.def)) mixBaseColor = colorScheme.getColor(tex);
				instance.GetComponent<Cloneable>().setColor(mixColors(mixBaseColor, colorScheme.getColor(tex)));
				instance.GetComponent<UnitBehaviour> ().SetSelect (false);
				instance.GetComponent<UnitBehaviour> ().SetCircleId(GetComponent<UnitBehaviour>().id + 1);

			}
		}
	}

	public void setColor(Color c) {
		currentColor = c;
	}

	private Color mixColors(Color a, Color b) {
		return new Color (
			(a.r + b.r) / 2,
			(a.g + b.g) / 2,
			(a.b + b.b) / 2,
			(a.a + b.a) / 2
		);
	}

	private float getElevation() {
		try {
			return FindObjectOfType<TerrainGenerator> ().getElevationForPoint (((int)transform.position.x), ((int)transform.position.z));
		} catch {
			return ((float)Random.Range (1, 100)) / 100f;
		}
	}
}
