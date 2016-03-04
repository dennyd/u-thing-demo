using UnityEngine;
using System.Collections;

[System.Serializable]
public class PrefabPreset {

	public GameObject terrain;
	public GameObject camera;
	public GameObject lightSet;
	public GameObject unitPrefab;

}

[System.Serializable]
public class MapPreset {

	public int size = 128, height = 15;
	public bool randomMap = true;

}

[System.Serializable]
public class GamePreset {

	public int numberOfRegions = 10;

	public int playerNumber = 2;
	public int neutralUnits = 20;

	public int aiSpawnInterval = 20;
	public int maxUnits = 10;

}

[System.Serializable]
public class AdvancedPreset {

	public int neutralUnitsEdgeLimit = 15;
	public Vector2 playerUnitSpawnPlace = new Vector2(5, 5);

}
	

public class GameController : MonoBehaviour {
	// Use this for initialization

	public PrefabPreset prefabPresets = new PrefabPreset ();
	public MapPreset mapSettings = new MapPreset ();
	public GamePreset gameSettings = new GamePreset ();
	public AdvancedPreset advancedSettings = new AdvancedPreset();

	private TerrainGenerator _t;
	void Start () {
		
		GameObject _c, _l;

		gameSettings.playerNumber = Mathf.Clamp (gameSettings.playerNumber, 2, 4);
		gameSettings.maxUnits = Mathf.Clamp (gameSettings.maxUnits, 0, mapSettings.size / 16);

		_t = ((GameObject) Instantiate (prefabPresets.terrain, Vector3.zero, Quaternion.identity)).GetComponent<TerrainGenerator>();
		_c = (GameObject) Instantiate (prefabPresets.camera, Vector3.zero, Quaternion.identity);
		_l = (GameObject) Instantiate (prefabPresets.lightSet, new Vector3(0, 100, 0), Quaternion.identity);

		_t.tag = "terrain";

		_t.width = mapSettings.size;
		_t.height = mapSettings.size;

		_t.t_resolution = mapSettings.size + 1;
		_t.t_heightmap_resolution = mapSettings.size + 1;

		_t.terrainHeight = mapSettings.height;

		_t.useRandomSeed = true;

		_t.noiseLevel = 500;
		_t.noiseScale = 20;

		_t.normalizationScale = 1;

		_t.liveGeneration = false;
		_t.useRandomSeed = mapSettings.randomMap;

		_t.VoronoiPolygonNumber = gameSettings.numberOfRegions;
		_t.RegenerateAll ();

		_c.transform.position = new Vector3 (0, 35, 30);
		_c.transform.eulerAngles = new Vector3 (55, 90, 0);
		_c.transform.position = new Vector3 (-advancedSettings.playerUnitSpawnPlace.x, 35, advancedSettings.playerUnitSpawnPlace.y);

		gameObject.AddComponent<MovementController> ();

		for (int i = 0; i < gameSettings.neutralUnits; i++) {
			Instantiate (
				prefabPresets.unitPrefab, 
				new Vector3 (
					Random.Range (advancedSettings.neutralUnitsEdgeLimit, mapSettings.size - advancedSettings.neutralUnitsEdgeLimit), 
					mapSettings.height, 
					Random.Range (advancedSettings.neutralUnitsEdgeLimit, mapSettings.size - advancedSettings.neutralUnitsEdgeLimit)), 
				Quaternion.identity
			);
		}

		GameObject bound;

		bound = GameObject.CreatePrimitive (PrimitiveType.Quad);
		bound.transform.localScale = new Vector3 (mapSettings.size, mapSettings.height * 2, 1);
		bound.transform.position = new Vector3 (mapSettings.size / 2, mapSettings.height, 0);
		bound.transform.eulerAngles = new Vector3 (180, 0, 0);
		bound.GetComponent<MeshRenderer> ().enabled = false;
		bound.layer = 2;

		bound = GameObject.CreatePrimitive (PrimitiveType.Quad);
		bound.transform.localScale = new Vector3 (mapSettings.size, mapSettings.height * 2, 1);
		bound.transform.position = new Vector3 (mapSettings.size / 2, mapSettings.height, mapSettings.size);
		bound.GetComponent<MeshRenderer> ().enabled = false;
		bound.layer = 2;

		bound = GameObject.CreatePrimitive (PrimitiveType.Quad);
		bound.transform.localScale = new Vector3 (mapSettings.size, mapSettings.height * 2, 1);
		bound.transform.position = new Vector3 (0, mapSettings.height, mapSettings.size / 2);
		bound.transform.eulerAngles = new Vector3 (0, 90, 0);
		bound.GetComponent<MeshRenderer> ().enabled = false;
		bound.layer = 2;

		bound = GameObject.CreatePrimitive (PrimitiveType.Quad);
		bound.transform.localScale = new Vector3 (mapSettings.size, mapSettings.height * 2, 1);
		bound.transform.position = new Vector3 (mapSettings.size, mapSettings.height, mapSettings.size / 2);
		bound.transform.eulerAngles = new Vector3 (180, 90, 0);
		bound.GetComponent<MeshRenderer> ().enabled = false;
		bound.layer = 2;

		bound = GameObject.CreatePrimitive (PrimitiveType.Quad);
		bound.transform.localScale = new Vector3 (mapSettings.size, mapSettings.size, 1);
		bound.transform.position = new Vector3 (mapSettings.size / 2, mapSettings.height * 2, mapSettings.size / 2);
		bound.transform.eulerAngles = new Vector3 (90, 0, 0);
		bound.GetComponent<MeshRenderer> ().enabled = false;
		bound.layer = 2;

		GameObject unit;

		// Player 1
		unit = (GameObject) Instantiate (
			prefabPresets.unitPrefab, 
			new Vector3 (advancedSettings.playerUnitSpawnPlace.x, mapSettings.height, advancedSettings.playerUnitSpawnPlace.y),
			Quaternion.identity
		);

		unit.GetComponent<UnitBelonging> ().player = 1;

		// Player 2
		unit = (GameObject) Instantiate (
			prefabPresets.unitPrefab, 
			new Vector3 (mapSettings.size - advancedSettings.playerUnitSpawnPlace.x, mapSettings.height, mapSettings.size - advancedSettings.playerUnitSpawnPlace.y),
			Quaternion.identity
		);

		unit.GetComponent<UnitBelonging> ().player = 2;
		SimpleExpansion ai = unit.AddComponent<SimpleExpansion> () as SimpleExpansion;

		ai.spawnInterval = gameSettings.aiSpawnInterval;
		ai.unitPreset = prefabPresets.unitPrefab;
		ai.mapSettings = mapSettings;

		if (gameSettings.playerNumber > 2) {

			// Player 3
			unit = (GameObject) Instantiate (
				prefabPresets.unitPrefab, 
				new Vector3 (mapSettings.size - advancedSettings.playerUnitSpawnPlace.x, mapSettings.height, advancedSettings.playerUnitSpawnPlace.y),
				Quaternion.identity
			);

			unit.GetComponent<UnitBelonging> ().player = 3;
			ai = unit.AddComponent<SimpleExpansion> () as SimpleExpansion;

			ai.spawnInterval = gameSettings.aiSpawnInterval;
			ai.unitPreset = prefabPresets.unitPrefab;
			ai.mapSettings = mapSettings;
		}

		if (gameSettings.playerNumber > 3) {

			// Player 4
			unit = (GameObject) Instantiate (
				prefabPresets.unitPrefab, 
				new Vector3 (advancedSettings.playerUnitSpawnPlace.x, mapSettings.height, mapSettings.size - advancedSettings.playerUnitSpawnPlace.y),
				Quaternion.identity
			);

			unit.GetComponent<UnitBelonging> ().player = 4;
			ai = unit.AddComponent<SimpleExpansion> () as SimpleExpansion;

			ai.spawnInterval = gameSettings.aiSpawnInterval;
			ai.unitPreset = prefabPresets.unitPrefab;
			ai.mapSettings = mapSettings;
		}

	}

	public bool canClone(UnitBelonging owner) {
		Cloneable[] units = FindObjectsOfType<Cloneable> ();

		int number = 0;
		foreach (Cloneable unit in units) {
			if (unit.GetComponent<UnitBelonging> ().isSamePlayerAs (owner))
				number += 1;
		}

		return number < gameSettings.maxUnits;
	}
}
