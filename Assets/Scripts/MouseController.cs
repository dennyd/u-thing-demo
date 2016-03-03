using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using csDelaunay;

public class MouseController : MonoBehaviour
{

	public Vector3[] newVertices;
	public Vector2[] newUV;
	public int[] newTriangles;

	void Start ()
	{
		Mesh mesh = new Mesh ();
		GetComponent<MeshFilter> ().mesh = mesh;
		mesh.vertices = newVertices;
		mesh.uv = newUV;
		mesh.triangles = newTriangles;
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


		// MOUSE _ TERITORY

//		if (collider.Raycast (ray, out hitInfo, Mathf.Infinity)) {
////			Debug.Log ( transform.worldToLocalMatrix *  hitInfo.point);
//			int x = (int)Mathf.Floor ((transform.worldToLocalMatrix * hitInfo.point).x);
//			int y = (int)Mathf.Floor ((transform.worldToLocalMatrix * hitInfo.point).z);
////			Debug.Log ("X: " + x + " Y: " + y);
//
//			GameObject terrainGO = GameObject.FindGameObjectWithTag ("terrain");
////			terrainGen<TerrainGenerator> ();
//			TerrainGenerator tg = terrainGO.GetComponent<TerrainGenerator> ();
//			List<Texture2D> vorTex = tg.voronoiTexList;
//			Voronoi vorDiagram = tg.voronoiDiagram;
//
//			GameObject p = GameObject.FindGameObjectWithTag ("projector");
//			Projector projector = p.GetComponent<Projector> ();
//
//		
//
//			Dictionary<Vector2f,Site> sites = vorDiagram.SitesIndexedByLocation;
//			float dist = Mathf.Infinity;			 
//			Vector2f closest = new Vector2f(-1,-1);
//
//
//			foreach (KeyValuePair<Vector2f,Site> kv in sites) {
//				
//				Vector3 temp1 = new Vector3 (kv.Key.x,0,kv.Key.y);
//				Vector3 temp2 = new Vector3 (x,0,y);
//				float d = Vector3.Distance (temp1,temp2);
//
//				if (d < dist) {
//					dist = d;
//					closest = new Vector2f(kv.Key.x,kv.Key.y);
//				}
//
//			}
//
//			// closest
//
//			if (closest.x > -1) {
//				int index = vorDiagram.Regions ().IndexOf (vorDiagram.Region(closest));
//				Texture2D tex = vorTex [index];
//				tex.Apply ();
//				projector.material.mainTexture = tex; 
//
//			}
//
//
//
//				
//		}

		if (collider.Raycast (ray, out hitInfo, Mathf.Infinity)) {

			int x = (int)Mathf.Floor ((transform.worldToLocalMatrix * hitInfo.point).x);
			int y = (int)Mathf.Floor ((transform.worldToLocalMatrix * hitInfo.point).z);



			if (Input.GetMouseButtonDown (0)) {

				GameObject terrainGO = GameObject.FindGameObjectWithTag ("terrain");
				TerrainGenerator tg = terrainGO.GetComponent<TerrainGenerator> ();

				List<Texture2D> vorTex = tg.voronoiTexList;
				List<Texture2D> vorTexP1 = tg.voronoiTexListPlayer1;
				List<Texture2D> vorTexP2 = tg.voronoiTexListPlayer2;

				Voronoi vorDiagram = tg.voronoiDiagram;
				List<Vector2f> regionToSite = tg.regionToSite;

				int width = tg.width;
				int height = tg.height;

				GameObject p = GameObject.FindGameObjectWithTag ("projector");
				Projector projector = p.GetComponent<Projector> ();

				int region = RegionOfPoint (new Vector2f (x, y), vorDiagram);
				Debug.Log ("region: " + region);


//				if (tg.regionToOwnerMap [region] == 1) {
//					vorTex = vorTexP1;
//				}
//
//				if (tg.regionToOwnerMap [region] == 2) {
//					vorTex = vorTexP2;
//				}
					
//
//				Texture2D tex = vorTex [region];
//				Texture2D tex1P = vorTexP1 [region];
//				Texture2D tex2P = vorTexP2 [region];


				Texture2D texCombined = new Texture2D (width, height);
//				Texture2D texCombined = tex1P;

				// CHANGE 
//				tg.ComputePointToOwnerMap (tg.regionToOwnerMap,tg.pointToRegionMap);
						
				tg.regionToOwnerMap [region] = 2;
		
				tg.pointToOwnerMap = tg.ComputePointToOwnerMap (tg.regionToOwnerMap,tg.pointToRegionMap);



				for (int i = 0; i < width; i++) {
					for (int j = 0; j < height; j++) {												
						Color p1 = new Color (0, 1, 0, 0.3f);
						Color p2 = new Color (1, 0, 0, 0.3f);
						Color c = new Color (0, 0, 0, 1.0f);

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
//				tex.Apply ();
//				projector.material.mainTexture = tex; 
				projector.material.mainTexture = texCombined; 



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







}
