using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using csDelaunay;
public class terGen : MonoBehaviour {

	public Vector3[] newVertices;
	public Vector2[] newUV;
	public int[] newTriangles;

	void Start() {
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = newVertices;
		mesh.uv = newUV;
		mesh.triangles = newTriangles;
	}

	
	// Update is called once per frame
	void Update () {
	

		GameObject terrainGO = GameObject.FindGameObjectWithTag ("terrain");
		TerrainGenerator tg = terrainGO.GetComponent<TerrainGenerator> ();
		List<Texture2D> vorTex = tg.voronoiTexList;
		Voronoi vorDiagram = tg.voronoiDiagram;
		List<Vector2f> regionToSite = tg.regionToSite;
		int width=tg.width;
		int height=tg.height;


		GameObject p = GameObject.FindGameObjectWithTag ("projector");
		Projector projector = p.GetComponent<Projector> ();



//		int region = RegionOfPoint (new Vector2f(x,y),vorDiagram);
		int region =0;



//		Texture2D tex = vorTex [region];
//		tex.Apply ();
//		projector.material.mainTexture = tex; 


		//
		List<LineSegment> segments = vorDiagram.VoronoiBoundarayForSite (regionToSite[region]);

		newVertices = new Vector3[segments.Count*2];
		newUV = new Vector2[] {new Vector2(0,256),new Vector2(256,256),new Vector2(256,0),new Vector2(0,0)};
		newTriangles = new int[] {0,1,2,0,2,3};

		int c=0;
		foreach (LineSegment segment in segments){
//			Debug.Log ("Line: " + segment.p0 + segment.p1);
			newVertices[c++]=new Vector3(segment.p0.x,200,segment.p0.y);
			newVertices[c++]=new Vector3(segment.p1.x,200,segment.p1.y);
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
