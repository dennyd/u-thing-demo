using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
public class MapGenerator : MonoBehaviour
{


	private Mesh mesh;
	private MeshCollider meshCollider;

	public int width;
	public int height;

	public string seed;
	public bool useRandomSeed;

	[Range (0, 100)]
	public int randomFillPercent;

	int[,] map;

	private Vector3[] vertices;


	void Start ()
	{
		GenerateMap ();
		mesh = new Mesh ();
		GetComponent<MeshFilter> ().mesh = mesh;
		mesh.name = "Procedural Grid";

		vertices = new Vector3[(width + 1) * (height + 1)];


		for (int v = 0, i = 0; i <= width; i++) {
			for (int j = 0; j <= height; j++, v++) {
				vertices [v] = new Vector3 (i, 0, j);
			}
		}


		mesh.vertices = vertices;

	
		int[] triangles = new int[(width) * (height) * 6];

		for (int t = 0, v = 0, j = 0; j < width; j++, v++) {

			for (int i = 0; i < height; i++,t += 6, v++) {

				triangles [t] = v;
				triangles [t + 1] = v + 1;
				triangles [t + 2] = v + height + 1;

				triangles [t + 3] = v + 1;
				triangles [t + 4] = v + height + 2;
				triangles [t + 5] = v + height + 1;
			}

		}
			

		mesh.triangles = triangles;


		Debug.Log (vertices.Length);
		Debug.Log (width*height);

		Vector3[] vert = mesh.vertices;
		//coloring
		Color[] colors = new Color[vert.Length];
		for (int i = 0; i < vert.Length; i++) {
			Vector3 v = vert[i];

			Debug.Log ((int)Math.Floor( v.x) + " " + ((int) Math.Floor(v.z)));

			int x = (int)Math.Floor (v.x);
			int z = (int)Math.Floor (v.z);
			if(x>=0 && x< width && z>=0 && z<height){

				if (map[x,z] == 1) {
					colors[i] = Color.Lerp(Color.red, Color.green, 0.3f);
				} else {
					colors[i] = Color.Lerp(Color.red, Color.green, 0.7f);
				}
		
			}



		}

		mesh.colors = colors;



		MeshCollider mc = gameObject.AddComponent<MeshCollider> ();
		mc.sharedMesh = mesh;

		// texture, uvs etc
		int tileAmount = 6;
		float squareSize = 2;
		Vector2[] uvs = new Vector2[vertices.Length];
		for (int i =0; i < vertices.Length; i ++) {
			float percentX = Mathf.InverseLerp(-map.GetLength(0)/2*squareSize,map.GetLength(0)/2*squareSize,vertices[i].x) * tileAmount;
			float percentY = Mathf.InverseLerp(-map.GetLength(0)/2*squareSize,map.GetLength(0)/2*squareSize,vertices[i].z) * tileAmount;
			uvs[i] = new Vector2(percentX,percentY);
		}
		mesh.uv = uvs;



	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			GenerateMap ();
		}
	}

	void GenerateMap ()
	{
		map = new int[width, height];
		RandomFillMap ();

		for (int i = 0; i < 5; i++) {
			SmoothMap ();
		}
	}


	void RandomFillMap ()
	{
		if (useRandomSeed) {
			seed = Time.time.ToString ();
		}

		System.Random pseudoRandom = new System.Random (seed.GetHashCode ());

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
					map [x, y] = 1;
				} else {
					map [x, y] = (pseudoRandom.Next (0, 100) < randomFillPercent) ? 1 : 0;
				}
			}
		}
	}

	void SmoothMap ()
	{
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				int neighbourWallTiles = GetSurroundingWallCount (x, y);

				if (neighbourWallTiles > 4)
					map [x, y] = 1;
				else if (neighbourWallTiles < 4)
					map [x, y] = 0;

			}
		}
	}

	int GetSurroundingWallCount (int gridX, int gridY)
	{
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {
				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
					if (neighbourX != gridX || neighbourY != gridY) {
						wallCount += map [neighbourX, neighbourY];
					}
				} else {
					wallCount++;
				}
			}
		}

		return wallCount;
	}



	//
	//	void OnDrawGizmos() {
	//		if (map != null) {
	//			for (int x = 0; x < width; x ++) {
	//				for (int y = 0; y < height; y ++) {
	//					Gizmos.color = (map[x,y] == 1)?Color.black:Color.white;
	//					Vector3 pos = new Vector3(-width/2 + x + .5f,0, -height/2 + y+.5f);
	//					Gizmos.DrawCube(pos,Vector3.one);
	//				}
	//			}
	//		}
	//	}
	//


	//	void OnDrawGizmos () {
	//
	//		if (vertices == null) {
	//			return;
	//		}
	//
	//		Gizmos.color = Color.white;
	//		for (int i = 0; i < vertices.Length; i++) {
	//			Gizmos.DrawSphere(vertices[i], 0.1f);
	//		}
	//	}

}