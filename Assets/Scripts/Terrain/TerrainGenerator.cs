using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Collections.Generic;

using csDelaunay;


[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{	



	public int width;
	public int height;
	public int terrainHeight;
	public bool useRandomSeed = true;
	public bool liveGeneration = true;


	public int t_heightmap_resolution;
	public int t_resolution;


	public float noiseLevel;
	public float noiseScale;
	//[Range (0, 400)]
	//public float meshScale;

	private float[,] map;
	private float mapLowestVal = 0;
	private float mapHighestVal = 0;

	public float normalizationScale;

	public Texture2D texture1;
	public Texture2D texture2;
	public Texture2D texture3;
	public Texture2D texture4;




	TerrainData terrainData;

	//voronoi
	public int VoronoiPolygonNumber = 10;
	private Dictionary<Vector2f, Site> sites;
	private List<Edge> edges;
	Texture2D voronoiTex;

	public Voronoi voronoiDiagram;
	public List<Texture2D> voronoiTexList;

	public int[,] pointToRegionMap;

	void Start ()
	{
		GenerateVoronoi ();

		GenerateMap ();
		GenerateTerrain ();
		//?

		GenerateTexture ();

	}

	public void GenerateTerrain()
	{

		int t_width = width;
		int t_height = height;

		if (terrainData == null) {
			terrainData = new TerrainData();
		}

		terrainData.heightmapResolution = t_heightmap_resolution;
		terrainData.alphamapResolution = t_resolution;



//		terrainData.size = new Vector3( t_width/ 16f, terrainHeight, t_height/ 16f );
		terrainData.size = new Vector3( t_width, terrainHeight, t_height );

		terrainData.SetHeights (0, 0, map);
		GameObject activeTerrain;
		if (Terrain.activeTerrain != null) {
//			Debug.Log ("> Active terrain exists");
			Terrain t = Terrain.activeTerrain;
			t.terrainData = terrainData;

		} else {
//			Debug.Log ("> No active terrain");
			Terrain.CreateTerrainGameObject (terrainData);
			Terrain t = Terrain.activeTerrain;
			t.terrainData = terrainData;
		}

		Terrain temp = Terrain.activeTerrain;
//		Debug.Log (temp.terrainData.size);


	}

	void GenerateMap ()
	{
		map = new float[width + 1, height + 1];
		float seed = 0.1337f;
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
				normalMap [x, y] *= normalizationScale;
			}
		}
		map = normalMap;
	}





	void GenerateNoise (float seed, float scale, float level)
	{
		//float border = 10.0f;
		for (int x = 0; x <= width; x++) {
			for (int y = 0; y <= height; y++) {
				if (x == 0 || x == width || y == 0 || y == height) {
					//map [x, y] = border;
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



	void ApplyTextures(){

//		Material mat = GetComponent<MeshRenderer> ().sharedMaterial;
//		mat.SetTexture ("_HeightMap2D", heightMap2D);

	}



	float Normalize (float val, float min, float max)
	{
		return (val - min) / (max - min);
	}


	void GenerateTexture(){


		/*
		 * Generating spalt maps for terrain texturing 
		 *
		*/

		SplatPrototype splat1 = new SplatPrototype ();
		SplatPrototype splat2 = new SplatPrototype ();
		SplatPrototype splat3 = new SplatPrototype ();
		SplatPrototype splat4 = new SplatPrototype ();

		SplatPrototype splatVoronoi = new SplatPrototype ();

		splat1.texture = texture1;
		splat2.texture = texture2;
		splat3.texture = texture3;
		splat4.texture = texture4;

		splatVoronoi.texture = voronoiTex;
		splatVoronoi.tileSize = new Vector2(width,height);
//		splatVoronoi.tileSize = new Vector2(terrainData.size.x,terrainData.size.y);
//		Debug.Log (width + " , " + height);
//		Debug.Log (terrainData.size.x + " , " +terrainData.size.y);


		terrainData.splatPrototypes = new SplatPrototype[] {
			splat1, 
			splat2, 
			splat3, 
			splat4,
			splatVoronoi
		};


		terrainData.RefreshPrototypes();

		float[,,] splatMap = new float[terrainData.alphamapResolution, terrainData.alphamapResolution, terrainData.splatPrototypes.Length];

		for (var x = 0; x < terrainData.alphamapWidth; x++)
		{
			for (var y = 0; y < terrainData.alphamapHeight; y++)
			{
				// voronoi tex
				splatMap [x, y,4] = 0.3f;

				if (map [x, y] > 0.9) {
					splatMap [x, y, 0] = 1;
					splatMap [x, y, 1] = 0;
					splatMap [x, y, 2] = 0;
					splatMap [x, y, 3] = 0;

				} else if (map [x, y] > 0.7) {
//					splatMap [x, y, 0] = map [x, y];
//					splatMap [x, y, 1] = 1 - map [x, y];
//


					/*
					 * 
					 *  [0.5,...,0.7]
					 * 
					 * 	0.5
					 * 		t2 -> 1
					 * 		t1 -> 0
					 * 
					 * 0.7 
					 * 		t2 -> 0
					 * 		t1 -> 1
					 * 
					 * 
					 * 
					 * */


//					float t1 = Mathf.Lerp (0,1,map [x, y]);
//					float t2 = Mathf.Lerp (1,0,map [x, y]);
					float min=0.7f;
					float max = 0.9f;
					float t1 = 1- Normalize(map [x, y],max,min);
					float t2 = 1 - t1;

//					Debug.Log (t1 + " , " +t2 );
					splatMap [x, y, 0] = t1;
					splatMap [x, y, 1] = t2;


					splatMap [x, y, 2] = 0;
					splatMap [x, y, 3] = 0;


				} else if (map [x, y] > 0.5) {
					splatMap [x, y, 0] = 0;
					splatMap [x, y, 1] = map [x, y];
					;
					splatMap [x, y, 2] = 1 - map [x, y];
					;
					splatMap [x, y, 3] = 0;
				} else if (map [x, y] > 0.3) {
					splatMap [x, y, 0] = 0;
					splatMap [x, y, 1] = map [x, y];
					splatMap [x, y, 2] = 1;
					splatMap [x, y, 3] = 0;
					;
				} else if (map [x, y] > 0.1) {
					splatMap [x, y, 0] = 0;
					splatMap [x, y, 1] = 0;
					splatMap [x, y, 2] = 1;
					splatMap [x, y, 3] = 0;
				} else {
					splatMap [x, y, 0] = 0;
					splatMap [x, y, 1] = 0;
					splatMap [x, y, 2] = 0;
					splatMap [x, y, 3] = 1;
				}


				 


			}
		}

		terrainData.SetAlphamaps(0, 0, splatMap);


	}

	public Boolean mouseClickRegenerate = false;
	void Update ()
	{
		if (mouseClickRegenerate && Input.GetMouseButtonDown (1)) {
			GenerateMap ();
			GenerateTerrain ();
			GenerateVoronoi();
			GenerateTexture ();
		}


		if(liveGeneration){

			GenerateMap ();
			GenerateTerrain ();
			//?
//			GenerateVoronoi();
			GenerateTexture ();

		}


	}


	void GenerateVoronoi(){
		
		List<Vector2f> points = new List<Vector2f>();
		for (int i = 0; i < VoronoiPolygonNumber; i++) {
//			points.Add(new Vector2f(UnityEngine.Random.Range(0,512), UnityEngine.Random.Range(0,512)));
			points.Add(new Vector2f(UnityEngine.Random.Range(0,width), UnityEngine.Random.Range(0,height)));
		}
//		Rectf bounds = new Rectf(0,0,512,512);
		Rectf bounds = new Rectf(0,0,width,height);
		Voronoi voronoi = new Voronoi(points,bounds,5);
		voronoiDiagram = voronoi;

		sites = voronoi.SitesIndexedByLocation;
		edges = voronoi.Edges;



//		Texture2D tx = new Texture2D(512,512);
		Texture2D tx = new Texture2D(width,height);

		for (int i=0; i<width; i++){
			for (int j=0; j<height; j++){
				tx.SetPixel (i, j, Color.black);
			}	
		}

		foreach (KeyValuePair<Vector2f,Site> kv in sites) {
			tx.SetPixel((int)kv.Key.x, (int)kv.Key.y, Color.red);
		}

		// draw voronoi on textures
		foreach (Edge edge in edges) {
			// if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
			if (edge.ClippedEnds == null) continue;
			DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], tx, Color.white);
		}
//
		//Debug.Log ("Counter: " + couter);
		tx.Apply();
		voronoiTex = tx;



		// PROJECTOR

		GameObject p =  GameObject.FindGameObjectWithTag ("projector");
		Projector projector = p.GetComponent<Projector> ();
		projector.transform.position = new Vector3 (width/2,terrainHeight*2,height/2);
		projector.transform.rotation = Quaternion.Euler(90,0,0);


//		Material mat = p.GetComponents<Material> ()[0];
		var mat = new Material (Shader.Find("Particles/Additive"));
//		p.GetComponents<Material> ()[0] = mat;
		projector.material = mat;
		projector.orthographicSize = 128;

		long t1 = System.DateTime.Now.Millisecond;
		pointToRegionMap = ComputePointToRegionArray (voronoiDiagram);
//		Debug.Log("time for ComputePointToRegionArray: "+ (System.DateTime.Now.Millisecond - t1));
		Texture2D fst = new Texture2D(width,height);
			
		for (int i=0; i<width; i++){
			for (int j=0; j<height; j++){
				fst.SetPixel (i, j, new Color(0,0,0,0));
			}	
		}

		foreach (Edge edge in edges) {
			// if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
			if (edge.ClippedEnds == null) continue;
			DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], fst, new Color(1,1,1,0.5f));
		}

		voronoiTexList = new List<Texture2D> ();



		for (int i = 0; i < VoronoiPolygonNumber; i++) {
			Texture2D tex = new Texture2D (width, height);
			voronoiTexList.Add (tex);
		}


		for (int i = 0; i < voronoi.Regions().Count; i++) {
			List<Vector2f> region = voronoi.Regions()[i];
			Texture2D tex = voronoiTexList [i];
			Color[] pixArray = tex.GetPixels ();
			t1 = System.DateTime.Now.Millisecond;

			for(int t=0; t<pixArray.Length; t++){
				// de-flattering
				int y = (int) Math.Floor ( t / width  *1.0f );
				int x = t - (y * width);

				pixArray [t] = new Color (0, 0, 0, 0);

//				if (RegionOfPoint(new Vector2f(x,y),voronoi)==i){
////					tex.SetPixel (x, y, new Color(1,1,1,0.3f));
//					pixArray [t] = new Color(1,1,1,0.3f);
//				}
				if (pointToRegionMap[x,y]==i){
					//					tex.SetPixel (x, y, new Color(1,1,1,0.3f));
					pixArray [t] = new Color(1,1,1,0.3f);
				}
//				Debug.Log("time sub: "+ (System.DateTime.Now.Millisecond - t2));


			}

			Debug.Log("time for 1 tex: "+ (System.DateTime.Now.Millisecond - t1));
			tex.SetPixels (pixArray);
//			for (int x=0; x<width; x++){
//				for (int y=0; y<height; y++){
//					tex.SetPixel (x, y, new Color(0,0,0,0));
//
//					if (RegionOfPoint(new Vector2f(x,y),voronoi)==i){
//						tex.SetPixel (x, y, new Color(1,1,1,0.3f));
//					}
//				}	
//			}

			tex.Apply ();

		}
			

		fst.Apply ();
		mat.mainTexture = fst;


	}





	// supporting methods
	// Bresenham line algorithm
	private void DrawLine(Vector2f p0, Vector2f p1, Texture2D tx, Color c, int offset = 0) {
		int x0 = (int)p0.x;
		int y0 = (int)p0.y;
		int x1 = (int)p1.x;
		int y1 = (int)p1.y;

		int dx = Mathf.Abs(x1-x0);
		int dy = Mathf.Abs(y1-y0);
		int sx = x0 < x1 ? 1 : -1;
		int sy = y0 < y1 ? 1 : -1;
		int err = dx-dy;

		while (true) {
			tx.SetPixel(x0+offset,y0+offset,c);

			if (x0 == x1 && y0 == y1) break;
			int e2 = 2*err;
			if (e2 > -dy) {
				err -= dy;
				x0 += sx;
			}
			if (e2 < dx) {
				err += dx;
				y0 += sy;
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
			
		if (closest.x > -1) {
			int index = vd.Regions ().IndexOf (vd.Region(closest));
			return index;
		}

		return -1;
	}

	int[,] ComputePointToRegionArray(Voronoi vd){

		int width = this.width;
		int height = this.height;

		int[,] outArr = new int[width,height];


		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				outArr [x,y] = RegionOfPoint (new Vector2f(x,y),vd);

			}
		}

		return outArr;	
	}

	public float getElevationForPoint(int x, int y) {
		if (map != null) return map [y, x];
		return -1;
	}

}