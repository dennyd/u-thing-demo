using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using csDelaunay;

public class MouseController : MonoBehaviour
{



	void Start ()
	{

	}

	//
	//	// Use this for initialization
	//	void Start ()
	//	{
	//
	//	}

	// Update is called once per frame
	void Update ()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitInfo;

		TerrainCollider collider = Terrain.activeTerrain.GetComponents<TerrainCollider> () [0];


		if (collider.Raycast (ray, out hitInfo, Mathf.Infinity)) {

			int x = (int)Mathf.Floor ((transform.worldToLocalMatrix * hitInfo.point).x);
			int y = (int)Mathf.Floor ((transform.worldToLocalMatrix * hitInfo.point).z);



			if (Input.GetMouseButtonDown (0)) {
				//				handleMouse0 (x, y);
			}

			if (Input.GetMouseButtonDown (3)) {
				//				handleMouse3 (x, y);
			}
		}

	}


	int RegionOfPoint (Vector2f p, Voronoi vd)
	{

		Dictionary<Vector2f,Site> sites = vd.SitesIndexedByLocation;
		float dist = Mathf.Infinity;			 
		Vector2f closest = new Vector2f (-1, -1);

		foreach (KeyValuePair<Vector2f,Site> kv in sites) {

			Vector3 temp1 = new Vector3 (kv.Key.x, 0, kv.Key.y);
			Vector3 temp2 = new Vector3 (p.x, 0, p.y);
			float d = Vector3.Distance (temp1, temp2);

			if (d < dist) {
				dist = d;
				closest = new Vector2f (kv.Key.x, kv.Key.y);
			}

		}

		// closest

		if (closest.x > -1) {
			int index = vd.Regions ().IndexOf (vd.Region (closest));
			return index;
		}

		return -1;
	}



	void handleMouse0(int x,int y){

		GameObject terrainGO = GameObject.FindGameObjectWithTag ("terrain");
		TerrainGenerator tg = terrainGO.GetComponent<TerrainGenerator> ();
		int width = tg.width;
		int height = tg.height;

		GameObject p = GameObject.FindGameObjectWithTag ("projector");
		Projector projector = p.GetComponent<Projector> ();

		int region = RegionOfPoint (new Vector2f (x, y), tg.voronoiDiagram);
		Debug.Log ("region: " + region);

		Texture2D texCombined = new Texture2D (width, height);						
		tg.regionToOwnerMap [region] = 2;		
		tg.pointToOwnerMap = tg.ComputePointToOwnerMap (tg.regionToOwnerMap,tg.pointToRegionMap);

		Texture2D mask = tg.vdTex;

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {	

				//						Color p1 = new Color (0, 1, 0, 0.3f);
				//						Color p2 = new Color (1, 0, 0, 0.3f);
				//						Color c = new Color (0, 0, 0, 1.0f);
				//
				//
				Color p1 = new Color (0 , 1 * mask.GetPixel(i,j).grayscale, 0, 1f);
				Color p2 = new Color (1* mask.GetPixel(i,j).grayscale, 0, 0, 1f);
				Color c = new Color (0, 0, 0, 0.5f);


				if (tg.pointToOwnerMap [i, j] == 1) {							

					texCombined.SetPixel (i, j, p1);	
				} else if (tg.pointToOwnerMap [i, j] == 2) {
					texCombined.SetPixel (i, j, p2);	
				} else {
					texCombined.SetPixel (i, j, c);
				}							
			}
		}
		texCombined.Apply ();

		projector.material.mainTexture = texCombined; 

	}
	void handleMouse3(int x, int y){

		GameObject terrainGO = GameObject.FindGameObjectWithTag ("terrain");
		TerrainGenerator tg = terrainGO.GetComponent<TerrainGenerator> ();
		int width = tg.width;
		int height = tg.height;

		GameObject p = GameObject.FindGameObjectWithTag ("projector");
		Projector projector = p.GetComponent<Projector> ();

		int region = RegionOfPoint (new Vector2f (x, y), tg.voronoiDiagram);
		Debug.Log ("region: " + region);

		Texture2D texCombined = new Texture2D (width, height);						
		tg.regionToOwnerMap [region] = 1;		
		tg.pointToOwnerMap = tg.ComputePointToOwnerMap (tg.regionToOwnerMap,tg.pointToRegionMap);

		Texture2D mask = tg.vdTex;

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {	
				Color p1 = new Color (0 , 1, 0, mask.GetPixel(i,j).grayscale);
				Color p2 = new Color (1, 0, 0,  mask.GetPixel(i,j).grayscale);
				Color c = new Color (0, 0, 0, 0.5f);

				if (tg.pointToOwnerMap [i, j] == 1) {							
					texCombined.SetPixel (i, j, p1);	
				} else if (tg.pointToOwnerMap [i, j] == 2) {
					texCombined.SetPixel (i, j, p2);	
				} else {
					texCombined.SetPixel (i, j, c);
				}							
			}
		}
		texCombined.Apply ();

		projector.material.mainTexture = texCombined; 


	}

	public void UpdateProjection(){
		//		long t1 = System.DateTime.Now.Millisecond;
		GameObject terrainGO = GameObject.FindGameObjectWithTag ("terrain");
		TerrainGenerator tg = terrainGO.GetComponent<TerrainGenerator> ();
		int width = tg.width;
		int height = tg.height;

		GameObject p = GameObject.FindGameObjectWithTag ("projector");
		Projector projector = p.GetComponent<Projector> ();

		Texture2D texCombined = new Texture2D (width, height);						
		Texture2D mask = tg.vdTex;



		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {	
				Color p1 = new Color (0 , 1, 0, mask.GetPixel(i,j).grayscale);
				Color p2 = new Color (1, 0, 0,  mask.GetPixel(i,j).grayscale);
				Color p3 = new Color (0.8f, 0.4f, 0f, (float) mask.GetPixel(i,j).grayscale);
				Color p4 = new Color (0.8f, 0f, 0.8f, (float) mask.GetPixel(i,j).grayscale);
				Color c = new Color (0.5f, 0.5f, 0.5f, 0f);
				c = Color.gray;
				c.a = 0.95f;
				if (tg.pointToOwnerMap [i, j] == 1) {							
					texCombined.SetPixel (i, j, p1);	
				} else if (tg.pointToOwnerMap [i, j] == 2) {
					texCombined.SetPixel (i, j, p2);	
				} else if (tg.pointToOwnerMap [i, j] == 3) {
					texCombined.SetPixel (i, j, p3);	
				} else if (tg.pointToOwnerMap [i, j] == 4) {
					texCombined.SetPixel (i, j, p4);	
				} else {
					texCombined.SetPixel (i, j, c);
				}							
			}
		}

		texCombined.Apply ();
		Texture2D bluured =  tg.Blur (texCombined,4);
		//		Debug.Log ("UpdateProjection time:" + (System.DateTime.Now.Millisecond-t1));
		projector.material.mainTexture = bluured; 
	}


}