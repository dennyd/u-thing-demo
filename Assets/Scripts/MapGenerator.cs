using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
public class MapGenerator : MonoBehaviour
{	

	private Mesh mesh;
	private MeshCollider meshCollider;

	public int width;
	public int height;

	public bool useRandomSeed = true;
	public bool liveGeneration = true;

	public float noiseLevel;
	public float noiseScale;
	[Range (0, 300)]
	public float meshScale;

	private float[,] map;
	private float mapLowestVal = 0;
	private float mapHighestVal = 0;

	Texture2D heightMap2D;

	private Vector3[] vertices;


	void Start ()
	{
		GenerateMap ();
		GenerateMesh ();
		GenerateTexture ();

	}



	void GenerateMap ()
	{
		map = new float[width + 1, height + 1];
		float seed = 0;
		if (useRandomSeed) {
			seed = Time.time;
		}

		GenerateNoise (seed, noiseScale, noiseLevel);
		NormaliseMap ();
	}


	void NormaliseMap ()
	{
		float[,] normalMap = new float[width + 1, height + 1];
		for (int x = 0; x <= width; x++) {
			for (int y = 0; y <= height; y++) {
				normalMap [x, y] = (map [x, y] - mapLowestVal) / (mapHighestVal - mapLowestVal);
			}
		}
		map = normalMap;
	}


	void GenerateNoise (float seed, float scale, float level)
	{
		float border = 10.0f;
		for (int x = 0; x <= width; x++) {
			for (int y = 0; y <= height; y++) {
				if (x == 0 || x == width || y == 0 || y == height) {
					map [x, y] = border;
				} else {
					// todo: optimization
					map [x, y] = (Mathf.PerlinNoise (seed + x / scale, seed + y / scale)) * level;

					if (map [x, y] > mapHighestVal) {
						mapHighestVal = map [x, y];
					}

					if (map [x, y] < mapLowestVal) {
						mapLowestVal = map [x, y];
					}
				}
			}
		}
	}

	void GenerateMesh ()
	{
		mesh = new Mesh ();
		GetComponent<MeshFilter> ().mesh = mesh;
		mesh.name = "Procedural Mesh";

		// creating vertices for the mesh

		vertices = new Vector3[(width + 1) * (height + 1)];
		for (int v = 0, i = 0; i <= width; i++) {
			for (int j = 0; j <= height; j++, v++) {
				float y = map [i, j] * meshScale;
				vertices [v] = new Vector3 (i, y, j);
			}
		}

		mesh.vertices = vertices;

		// triangulating mesh
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

		// addding colider to the mesh	
		MeshCollider mc = gameObject.GetComponent<MeshCollider> ();
		if (mc == null) {
			mc = gameObject.AddComponent<MeshCollider> ();
		}
		mc.sharedMesh = mesh;


		// UVs
		Vector2[] uvs = new Vector2[vertices.Length];
		for (int i = 0; i < vertices.Length; i++) {
			float percentX = Mathf.InverseLerp (0, width, vertices [i].x);
			float percentY = Mathf.InverseLerp (0, height, vertices [i].z);
			uvs [i] = new Vector2 (percentX, percentY);
		}
			
		mesh.uv = uvs;




		mesh.RecalculateBounds (); 
		mesh.RecalculateNormals ();

	}


	void ApplyTextures(){

		Material mat = GetComponent<MeshRenderer> ().sharedMaterial;
		mat.SetTexture ("_HeightMap2D", heightMap2D);

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


	float Normalize (float val, float min, float max)
	{
		return (val - min) / (max - min);
	}


	void GenerateTexture(){

		heightMap2D = new Texture2D(width, height);
		for (int x = 0; x < heightMap2D.width; x++) {
			for (int y = 0; y < heightMap2D.height; y++) {	
				Color color = Color.Lerp(Color.white, Color.black, map[x,y]);
				heightMap2D.SetPixel(x, y, color);
			}
		}
		heightMap2D.Apply();

		ApplyTextures ();

	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			GenerateMap ();
			GenerateMesh ();
		}


		if(liveGeneration){

			GenerateMap ();
			GenerateMesh ();
			GenerateTexture ();

		}


	}

}