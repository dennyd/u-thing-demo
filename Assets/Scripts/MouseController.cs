using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using csDelaunay;

public class MouseController : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitInfo;

		TerrainCollider collider = Terrain.activeTerrain.GetComponents<TerrainCollider> () [0];

		if (collider.Raycast (ray, out hitInfo, Mathf.Infinity)) {
//			Debug.Log ( transform.worldToLocalMatrix *  hitInfo.point);
			int x = (int)Mathf.Floor ((transform.worldToLocalMatrix * hitInfo.point).x);
			int y = (int)Mathf.Floor ((transform.worldToLocalMatrix * hitInfo.point).z);
//			Debug.Log ("X: " + x + " Y: " + y);

			GameObject terrainGO = GameObject.FindGameObjectWithTag ("terrain");
//			terrainGen<TerrainGenerator> ();
			TerrainGenerator tg = terrainGO.GetComponent<TerrainGenerator> ();
			List<Texture2D> vorTex = tg.voronoiTexList;
			Voronoi vorDiagram = tg.voronoiDiagram;

			GameObject p = GameObject.FindGameObjectWithTag ("projector");
			Projector projector = p.GetComponent<Projector> ();

		

			Dictionary<Vector2f,Site> sites = vorDiagram.SitesIndexedByLocation;
			float dist = Mathf.Infinity;			 
			Vector2f closest = new Vector2f(-1,-1);


			foreach (KeyValuePair<Vector2f,Site> kv in sites) {
				
				Vector3 temp1 = new Vector3 (kv.Key.x,0,kv.Key.y);
				Vector3 temp2 = new Vector3 (x,0,y);
				float d = Vector3.Distance (temp1,temp2);

				if (d < dist) {
					dist = d;
					closest = new Vector2f(kv.Key.x,kv.Key.y);
				}

			}

			// closest

			if (closest.x > -1) {
				int index = vorDiagram.Regions ().IndexOf (vorDiagram.Region(closest));
//				Debug.Log ("GOOOD " + index);

				Texture2D tex = vorTex [index];
				tex.Apply ();
				projector.material.mainTexture = tex; 

			}
				
		}






	}


	int RegionOfPoint(Vector2f p, Voronoi vd){

		Dictionary<Vector2f,Site> sites = vd.SitesIndexedByLocation;
		float dist = Mathf.Infinity;			 
		Vector2f closest = new Vector2f(-1,-1);

		foreach (KeyValuePair<Vector2f,Site> kv in sites) {

			Vector3 temp1 = new Vector3 (kv.Key.x,0,kv.Key.y);
			Vector3 temp2 = new Vector3 (p.x,0,p.y);
			float d = Vector3.Distance (temp1,temp2);

			if (d < dist) {
				dist = d;
				closest = new Vector2f(kv.Key.x,kv.Key.y);
			}

		}

		// closest

		if (closest.x > -1) {
			int index = vd.Regions ().IndexOf (vd.Region(closest));
			return index;
		}

		return -1;
	}

}
