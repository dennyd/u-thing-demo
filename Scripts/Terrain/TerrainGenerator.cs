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
	public List<Texture2D> voronoiTexListPlayer1;
	public List<Texture2D> voronoiTexListPlayer2;
	public Texture2D texComb;

	// maps
	public int[,] pointToRegionMap;
	public int[,] pointToOwnerMap;
	public int[] regionToOwnerMap;
	public List<Vector2f> regionToSite;
	public Texture2D vdTex;


	Material terrainMat;
	Texture2D terrainMapTex ;
	void Start ()


	{

		regionToOwnerMap = new int[VoronoiPolygonNumber];

		GenerateMap ();
		GenerateTerrain ();
		//?
		GenerateVoronoi ();

		GenerateTexture ();

	}

	public void GenerateTerrain ()
	{
		Material mat = new Material (Shader.Find ("Custom/ToonTerrainFirstPass"));
		// map to tex2d
		Texture2D mapTex = new Texture2D(width,height);
		terrainMapTex = mapTex;
		for (var x = 0; x < width; x++) {
			for (var y = 0; y < height; y++) {
				float grad=0f;
				float off= 0.1f;
				mapTex.SetPixel (x, y, new Color (0.5f + ((map[x,y]*grad))-off , 
					0.5f + ((map[x,y]*grad))-off,
					0.5f + ((map[x,y]*grad))-off,0.5f));

//				mapTex.SetPixel (x, y, Color.white);

			}
		}
		mapTex.Apply ();
		mat.SetTexture ("_Ramp",mapTex);


		terrainMat = mat;
//		var mat = new Material (Shader.Find ("Custom/Terrain/Diffuse"));

		mat.SetTexture("_MainTex" , new Texture2D(width,height)); //set the texture properties of the shader
		mat.SetTexture("_MainTex2", new Texture2D(width,height));
		mat.SetTexture("_MainTex3", new Texture2D(width,height));
		mat.SetTexture("_MainTex4", new Texture2D(width,height));
		//mat.SetTexture ("_Ramp");
		int t_width = width;
		int t_height = height;

		if (terrainData == null) {
			terrainData = new TerrainData ();
		}

		terrainData.thickness = 20;

		terrainData.heightmapResolution = t_heightmap_resolution;
		terrainData.alphamapResolution = t_resolution;

		//		terrainData.size = new Vector3( t_width/ 16f, terrainHeight, t_height/ 16f );
		terrainData.size = new Vector3 (t_width, terrainHeight, t_height);

		terrainData.SetHeights (0, 0, map);
		GameObject activeTerrain;
		if (Terrain.activeTerrain != null) {
			//			Debug.Log ("> Active terrain exists");
			Terrain t = Terrain.activeTerrain;

			t.materialType = Terrain.MaterialType.Custom;
			t.materialTemplate = mat;

			t.terrainData = terrainData;

		} else {
			Terrain.CreateTerrainGameObject (terrainData);
			Terrain t = Terrain.activeTerrain;

			t.materialType = Terrain.MaterialType.Custom;
			t.materialTemplate = mat;


			t.terrainData = terrainData;
		}

		Terrain temp = Terrain.activeTerrain;
		//		Debug.Log (temp.terrainData.size);


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



	void ApplyTextures ()
	{

		//		Material mat = GetComponent<MeshRenderer> ().sharedMaterial;
		//		mat.SetTexture ("_HeightMap2D", heightMap2D);

	}



	float Normalize (float val, float min, float max)
	{
		return (val - min) / (max - min);
	}


	void GenerateTexture ()
	{


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
		splatVoronoi.tileSize = new Vector2 (width, height);
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


		terrainData.RefreshPrototypes ();

		float[,,] splatMap = new float[terrainData.alphamapResolution, terrainData.alphamapResolution, terrainData.splatPrototypes.Length];

		for (var x = 0; x < terrainData.alphamapWidth; x++) {
			for (var y = 0; y < terrainData.alphamapHeight; y++) {
				// voronoi tex
				splatMap [x, y, 4] = 0.3f;

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
					float min = 0.7f;
					float max = 0.9f;
					float t1 = 1 - Normalize (map [x, y], max, min);
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

		terrainData.SetAlphamaps (0, 0, splatMap);


	}

	public bool mousebtn1=false;
	void Update ()
	{
		if(mousebtn1){
			if (Input.GetMouseButtonDown (1)) {
				GenerateMap ();
				GenerateTerrain ();
				GenerateVoronoi ();
				GenerateTexture ();

			}
		}



		if (liveGeneration) {

			GenerateMap ();
			GenerateTerrain ();
			//?
			//			GenerateVoronoi();
			GenerateTexture ();

		}



	}

	void GenerateZoneMeshes(Voronoi vd){

		//		vd.Edges.


	}


	void GenerateVoronoi ()
	{

		List<Vector2f> points = new List<Vector2f> ();
		for (int i = 0; i < VoronoiPolygonNumber; i++) {
			//			points.Add(new Vector2f(UnityEngine.Random.Range(0,512), UnityEngine.Random.Range(0,512)));
			points.Add (new Vector2f (UnityEngine.Random.Range (0, width), UnityEngine.Random.Range (0, height)));
		}
		//		Rectf bounds = new Rectf(0,0,512,512);
		Rectf bounds = new Rectf (0, 0, width, height);
		Voronoi voronoi = new Voronoi (points, bounds, 5);
		voronoiDiagram = voronoi;

		sites = voronoi.SitesIndexedByLocation;
		edges = voronoi.Edges;

		//re-compute maps
		regionToSite =  ComputeRegionToSite(voronoi);
		//		regionToOwnerMap = ComputeRegionToOwnerMap ();


		// goes 1st

		pointToRegionMap = ComputePointToRegionMap (voronoiDiagram); //2nd
		pointToOwnerMap = ComputePointToOwnerMap (regionToOwnerMap, pointToRegionMap); //3rd

		//		Texture2D tx = new Texture2D(512,512);
		Texture2D tx = new Texture2D (width, height);

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				tx.SetPixel (i, j, Color.black);
			}	
		}

		foreach (KeyValuePair<Vector2f,Site> kv in sites) {
			tx.SetPixel ((int)kv.Key.x, (int)kv.Key.y, Color.red);
		}




		// draw voronoi on textures
		foreach (Edge edge in edges) {
			// if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
			if (edge.ClippedEnds == null)
				continue;

			//			Color p1 = new Color (0, 1, 0, 0.5f);
			//			Color p2 = new Color (0, 0, 1, 0.5f);
			//			Color p3 = new Color (1, 0, 0, 0.5f);
			Color c = new Color (0.3f, 0.3f, 0.3f, 1f);
			c = Color.black;


			DrawLine (edge.ClippedEnds [LR.LEFT], edge.ClippedEnds [LR.RIGHT], tx, c);

			//			if (true) {
			//				for (int i = 0; i < tx.width; i++) {
			//					for (int j = 0; j < tx.height; j++) {
			//						float col = tx.GetPixel (i, j).grayscale;
			//
			//						if(i>0 && i<tx.width-1 && j>0 && j<tx.height-1){
			//							float sum = 
			//								tx.GetPixel (i - 1, j - 1).grayscale + tx.GetPixel (i + 1, j + 1).grayscale +
			//								tx.GetPixel (i - 1, j + 1).grayscale + tx.GetPixel (i + 1, j - 1).grayscale +
			//								tx.GetPixel (i - 1, j).grayscale + tx.GetPixel (i, j - 1).grayscale +
			//								tx.GetPixel (i + 1, j).grayscale + tx.GetPixel (i, j + 1).grayscale;
			//
			////							sum /= 8f;
			//
			//							if(sum>0.1f){
			//								tx.SetPixel (i, j, new Color (1, 1, 1, 1));
			//							}
			//
			//
			//
			//						}
			//
			//					}
			//				}
			//
			//
			//			}


			//			DrawLine2 (tx,edge.ClippedEnds [LR.LEFT].x,edge.ClippedEnds [LR.LEFT].y,edge.ClippedEnds [LR.RIGHT].x,edge.ClippedEnds [LR.RIGHT].y,c);

		}

		//		for (int i=0;i<VoronoiPolygonNumber;i++){
		//
		//			List<LineSegment> segments = voronoi.VoronoiBoundarayForSite (regionToSite[i]);
		//
		//
		//			foreach (LineSegment segment in segments){
		//				//				Debug.Log ("Line: " + segment.p0 + segment.p1);
		//
		//
		//				if (regionToOwnerMap[i]==2){
		//					DrawLine (segment.p0, segment.p1, tx, Color.red);
		//				}
		//
		//				if (regionToOwnerMap[i]==1){
		//					DrawLine (segment.p0, segment.p1, tx, Color.green);
		//				}
		//
		//			}
		//
		//
		//		}



		//Debug.Log ("Counter: " + couter);
		tx.Apply ();

		Texture2D tx_blur = Blur (tx,2);
		tx = tx_blur;
		voronoiTex = tx;
		vdTex = tx;



		// modify mat of terrain


		// map to tex2d
//		Texture2D mapTex = terrainMapTex;
//		for (var x = 0; x < width; x++) {
//			for (var y = 0; y < height; y++) {
//
//
//				Color old = tx.GetPixel(x, y);
//
//
//				if (old.grayscale >0){
//					Color c =terrainMapTex.GetPixel (x, y);
//					c.r *= (old.grayscale/2);
//					c.g *= (old.grayscale/2);;
//					c.b *= (old.grayscale/2);;
//					c.a *= (old.grayscale/2);;
//
//					c = Color.black;
//
//					mapTex.SetPixel (x, y, c);
//
//				}
//
//
//			
//			}
//		}
//
//		mapTex.Apply ();
//		terrainMat.SetTexture ("_Ramp",mapTex);

		// PROJECTOR


		GameObject p = GameObject.FindGameObjectWithTag ("projector");
		Projector projector = p.GetComponent<Projector> ();
		projector.transform.position = new Vector3 (width / 2, terrainHeight * 2, height / 2);
		projector.transform.rotation = Quaternion.Euler (90, 0, 0);


		//		Material mat = p.GetComponents<Material> ()[0];
		//		var mat = new Material (Shader.Find ("Particles/Additive"));
		var mat = new Material (Shader.Find ("Particles/Additive"));
		//		var mat = new Material (Shader.Find ("Projector/AdditiveTint"));
		//		p.GetComponents<Material> ()[0] = mat;
		projector.material = mat;
		projector.orthographicSize = 128;

		//		long t1 = System.DateTime.Now.Millisecond;
		//
		//
		//		Debug.Log ("time for ComputePointToRegionArray: " + (System.DateTime.Now.Millisecond - t1));
		Texture2D fst = new Texture2D (width, height);

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				fst.SetPixel (i, j, new Color (0, 0, 0, 0));
			}	
		}

		//		foreach (Edge edge in edges) {
		//			// if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
		//			if (edge.ClippedEnds == null)
		//				continue;
		//
		//			Color p1 = Color.green;
		//			Color p2 = Color.red;
		//			Color c = new Color (1, 1, 1, 0.7f);
		//
		//			//			if(pointToOwnerMap[ (int) Math.Floor( edge.ClippedEnds[LR.LEFT].x),(int)Math.Floor( edge.ClippedEnds[LR.LEFT].y)]==1){
		//			//				c = p1;
		//			//			}
		//			//
		//			//			if(pointToOwnerMap[(int)Math.Floor( edge.ClippedEnds[LR.LEFT].x),(int)Math.Floor( edge.ClippedEnds[LR.LEFT].y)]==2){
		//			//				c = p2;
		//			//			}
		//
		//
		//			//
		//			DrawLine (edge.ClippedEnds [LR.LEFT], edge.ClippedEnds [LR.RIGHT], fst, c);
		//
		//
		//		}

		voronoiTexList = new List<Texture2D> ();
		voronoiTexListPlayer1 = new List<Texture2D> ();
		voronoiTexListPlayer2 = new List<Texture2D> ();

		for (int i = 0; i < VoronoiPolygonNumber; i++) {
			voronoiTexList.Add (new Texture2D (width, height));
			voronoiTexListPlayer1.Add (new Texture2D (width, height));
			voronoiTexListPlayer2.Add (new Texture2D (width, height));
		}


		for (int i = 0; i < voronoi.Regions ().Count; i++) {
			List<Vector2f> region = voronoi.Regions () [i];

			Texture2D tex = voronoiTexList [i];
			Texture2D texP1 = voronoiTexListPlayer1 [i];
			Texture2D texP2 = voronoiTexListPlayer2 [i];

			Color[] pixArray = tex.GetPixels ();
			Color[] pixArrayP1 = texP1.GetPixels ();
			Color[] pixArrayP2 = texP2.GetPixels ();

			for (int t = 0; t < pixArray.Length; t++) {
				// de-flattering
				int y = (int)Math.Floor (t / width * 1.0f);
				int x = t - (y * width);

				pixArray [t] = new Color (0, 0, 0, 0);
				pixArrayP1[t] = new Color (0, 0, 0, 0);
				pixArrayP2[t] = new Color (0, 0, 0, 0);

				if (pointToRegionMap [x, y] == i) {
					pixArray [t] = new Color (1, 1, 1, 0.5f);
					pixArrayP1 [t] = new Color (1, 1, 1, 0.5f);
					pixArrayP2 [t] = new Color (1, 1, 1, 0.5f);
				}

			}				
			tex.SetPixels (pixArray);
			tex.Apply ();

			texP1.SetPixels (pixArrayP1);
			texP1.Apply ();

			texP2.SetPixels (pixArrayP2);
			texP2.Apply ();
		}

		//		vdTex = fst;
		fst.Apply ();
		mat.mainTexture = fst;


	}





	private void DrawLine (Vector2f p0, Vector2f p1, Texture2D tx, Color c, int offset = 0)
	{
		int x0 = (int)p0.x;
		int y0 = (int)p0.y;
		int x1 = (int)p1.x;
		int y1 = (int)p1.y;

		int dx = Mathf.Abs (x1 - x0);
		int dy = Mathf.Abs (y1 - y0);
		int sx = x0 < x1 ? 1 : -1;
		int sy = y0 < y1 ? 1 : -1;
		int err = dx - dy;

		while (true) {
			tx.SetPixel (x0 + offset, y0 + offset, c);
			tx.SetPixel (x0 + offset+1, y0 + offset+1, c);
			tx.SetPixel (x0 + offset-1, y0 + offset-1, c);
			tx.SetPixel (x0 + offset-1, y0 + offset+1, c);
			tx.SetPixel (x0 + offset+1, y0 + offset-1, c);
			tx.SetPixel (x0 + offset+1, y0 + offset, c);
			tx.SetPixel (x0 + offset-1, y0 + offset, c);
			tx.SetPixel (x0 + offset, y0 + offset+1, c);
			tx.SetPixel (x0 + offset, y0 + offset-1, c);
			if (x0 == x1 && y0 == y1)
				break;
			int e2 = 2 * err;
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

		if (closest.x > -1) {
			int index = vd.Regions ().IndexOf (vd.Region (closest));
			return index;
		}

		return -1;
	}

	int[,] ComputePointToRegionMap (Voronoi vd)
	{
		int width = this.width;
		int height = this.height;

		int[,] outArr = new int[width, height];


		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				outArr [x, y] = RegionOfPoint (new Vector2f (x, y), vd);

			}
		}
		return outArr;	
	}

	public int[,] ComputePointToOwnerMap (int[] regionToOwnerMap, int[,] pointToRegionMap)
	{

		int[,] map = new int[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				int region = pointToRegionMap [x, y];
				int owner = regionToOwnerMap [region];
				map [x, y] = owner;
			}
		}

		return map;
	}

	int[] ComputeRegionToOwnerMap ()
	{
		int[] map = new int[VoronoiPolygonNumber];



		for (int i = 0; i < VoronoiPolygonNumber; i++) {
			map [i] = 0;
		}
		//
		//		map [0] = 1;
		//		map [5] = 2;

		return map;
	}


	List<Vector2f> ComputeRegionToSite (Voronoi vd){
		List<Vector2f> o = new List<Vector2f>();
		Dictionary<Vector2f,Site> sites = vd.SitesIndexedByLocation;

		foreach (KeyValuePair<Vector2f,Site> kv in sites) {
			o.Add (kv.Key);
		}

		return o;
	}
	public float getElevationForPoint(int x, int y) {
		if (map != null) return map [y, x];
		return -1;
	}



	/* COPIED  */
	public Texture2D Blur(Texture2D image, int blurSize)
	{
		Texture2D blurred = new Texture2D(image.width, image.height);

		// look at every pixel in the blur rectangle
		for (int xx = 0; xx < image.width; xx++)
		{
			for (int yy = 0; yy < image.height; yy++)
			{
				float avgR = 0, avgG = 0, avgB = 0, avgA = 0;
				int blurPixelCount = 0;

				// average the color of the red, green and blue for each pixel in the
				// blur size while making sure you don't go outside the image bounds
				for (int x = xx; (x < xx + blurSize && x < image.width); x++)
				{
					for (int y = yy; (y < yy + blurSize && y < image.height); y++)
					{
						Color pixel = image.GetPixel(x, y);

						avgR += pixel.r;
						avgG += pixel.g;
						avgB += pixel.b;
						avgA += pixel.a;

						blurPixelCount++;
					}
				}

				avgR = avgR / blurPixelCount;
				avgG = avgG / blurPixelCount;
				avgB = avgB / blurPixelCount;
				avgA = avgA / blurPixelCount;

				// now that we know the average for the blur size, set each pixel to that color
				for (int x = xx; x < xx + blurSize && x < image.width; x++)
					for (int y = yy; y < yy + blurSize && y < image.height; y++)
						blurred.SetPixel(x, y, new Color(avgR, avgG, avgB, avgA));
			}
		}
		blurred.Apply();
		return blurred;
	}




}