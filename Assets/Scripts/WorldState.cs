using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldState : MonoBehaviour {

	int numberOfTerritories= 0;
	int[] territories;


	void Start () {
		GameObject terrainGO = GameObject.FindGameObjectWithTag ("terrain");
		TerrainGenerator tg = terrainGO.GetComponent<TerrainGenerator> ();
		numberOfTerritories = tg.VoronoiPolygonNumber;

		territories = new int[numberOfTerritories];

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
