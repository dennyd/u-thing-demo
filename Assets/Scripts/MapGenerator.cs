using UnityEngine;
using UnityEditor;
using System.Collections;
using System;


// texturing notes
// 512x512px with 2x2 tilnig for 20x20 units map
// wirking with ~ 50px for a unities unit





[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
public class MapGenerator : MonoBehaviour
{


	private Mesh mesh;
	private MeshCollider meshCollider;

	public int width;
	public int height;

//	public string seed;
	public bool useRandomSeed;

	//	[Range (0, 100)]
	//	public int randomFillPercent;
	//

	public float noiseLevel;
	public float noiseScale;
	[Range (0, 300)]
	public float meshScale;

	float[,] map;
	float mapLowestVal=0;
	float mapHighestVal=0;

	public Texture2D heatMapTexture;
	public Texture2D finalTexture;
	public Material mat;
	public Texture2D grass;
	public Texture2D rock;
	public Texture2D snow;
	public Texture2D water;

	public Texture2D grass_spread;
	public Texture2D rock_spread;
	public Texture2D snow_spread;
	public Texture2D water_spread;

	private Vector3[] vertices;


	void Start ()
	{
//		mat = new Material(Shader.Find("Standard"));
//		GetComponent<Renderer>().material = mat;



		string path = AssetDatabase.GetAssetPath(grass);
		TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
		textureImporter.isReadable = true;
		AssetDatabase.ImportAsset(path);

		Debug.Log ("Grass w: " + grass.width);
		Debug.Log ("Grass h: " + grass.height);

		path = AssetDatabase.GetAssetPath(water);
		textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
		textureImporter.isReadable = true;
		AssetDatabase.ImportAsset(path);

		path = AssetDatabase.GetAssetPath(snow);
		textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
		textureImporter.isReadable = true;
		AssetDatabase.ImportAsset(path);

		path = AssetDatabase.GetAssetPath(rock);
		textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
		textureImporter.isReadable = true;
		AssetDatabase.ImportAsset(path);



		GenerateMap ();
		GenerateMesh ();
		GenerateTexture ();




	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			GenerateMap ();
			GenerateMesh ();
			GenerateTexture ();
		}
	}

	void GenerateMap ()
	{
		map = new float[width+1, height+1];
		float seed = 0;
		if(useRandomSeed){
			seed = Time.time;
		}

		GenerateNoise (seed, noiseScale, noiseLevel);
		NormaliseMap ();

	}


	void NormaliseMap(){
		float[,] normalMap = new float[width+1, height+1];
		for (int x = 0; x <= width; x++) {
			for (int y = 0; y <= height; y++) {
				normalMap [x, y] = (map[x,y]-mapLowestVal)/(mapHighestVal-mapLowestVal);
			}
		}
		map = normalMap;
	}


	void GenerateNoise (float seed,float scale, float level)
	{
		float border = 10.0f;
		for (int x = 0; x <= width; x++) {
			for (int y = 0; y <= height; y++) {
				if (x == 0 || x == width || y == 0 || y == height) {
					map [x, y] = border;
				} else {

					// todo: optimization
					map [x, y] = (Mathf.PerlinNoise (seed + x/scale, seed+ y/scale)) * level;

					if(map [x, y]>mapHighestVal){
						mapHighestVal = map [x, y];
					}

					if(map [x, y]< mapLowestVal){
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
		mesh.name = "Procedural Grid";

		// creating vertices for the mesh

		vertices = new Vector3[(width + 1) * (height + 1)];
		for (int v = 0, i = 0; i <= width; i++) {
			for (int j = 0; j <= height; j++, v++) {
				float y = map[i,j] * meshScale;
				vertices [v] = new Vector3 (i, y, j);
			}
		}


		// set y-axis according to heat map

//		for (int x = 0; x < width; x++) {
//			for (int y = 0; y < height; y++) {
//				vertices [x * y].y = map [x, y];
//			}
//		}



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

		// mesh coloring
		Vector3[] vert = mesh.vertices;
		Color[] colors = new Color[vert.Length];
		for (int i = 0; i < vert.Length; i++) {
			Vector3 v = vert [i];

			int x = (int)Math.Floor (v.x);
			int z = (int)Math.Floor (v.z);
			if (x >= 0 && x < width && z >= 0 && z < height) {
//				if (map[x,z] == 1) {
//					colors[i] = Color.Lerp(Color.red, Color.green, 0.3f);
//				} else {
//					colors[i] = Color.Lerp(Color.red, Color.green, 0.7f);
//				}
				colors [i] = Color.Lerp (Color.blue, Color.red, map [x, z]);
			}
		}
//		mesh.colors = colors;

		// addding colider to the mesh	
		MeshCollider mc = gameObject.GetComponent<MeshCollider> ();
		if (mc == null) {
			mc = gameObject.AddComponent<MeshCollider> ();
		}

		mc.sharedMesh = mesh;

		// texture, uvs etc

		Vector2[] uvs = new Vector2[vertices.Length];
		for (int i = 0; i < vertices.Length; i++) {
			float percentX = Mathf.InverseLerp (0 , width, vertices [i].x);
			float percentY = Mathf.InverseLerp (0, height, vertices [i].z);
			uvs [i] = new Vector2 (percentX, percentY);
		}
			
		mesh.uv = uvs;

		mesh.RecalculateBounds (); 
		mesh.RecalculateNormals ();
	}



	void GenerateTexture(){
		
		heatMapTexture = new Texture2D(width, height);
		for (int x = 0; x < heatMapTexture.width; x++) {
			for (int y = 0; y < heatMapTexture.height; y++) {	
				Color color = Color.Lerp(Color.white, Color.black, map[x,y]);
				heatMapTexture.SetPixel(x, y, color);
			}
		}
		heatMapTexture.Apply();



		// 50px for unity unit
		int pixelsPerUnit =50;

		grass_spread = new Texture2D (grass.width* pixelsPerUnit,grass.height*pixelsPerUnit);

		// legacy block
		water_spread = new Texture2D (grass.width,grass.height);
		rock_spread = new Texture2D (grass.width,grass.height);
		snow_spread = new Texture2D (grass.width,grass.height);




		int tilingX=1;
		int tilingY=1;

		// spread texture prer
		for (int x = 0; x < grass_spread.width; x++) {
			for (int y = 0; y < grass_spread.height; y++) {
						
				grass_spread.SetPixel (x, y, grass.GetPixel (x%grass_spread.width, y%grass_spread.height ));

//				rock_spread.SetPixel (x, y, rock.GetPixel (x % rock.width, y % rock.height));
//				snow_spread.SetPixel (x, y, snow.GetPixel (x % snow.width, y % snow.height));
//				water_spread.SetPixel (x, y, water.GetPixel (x % water.width, y % water.height));
//
			}
		}

		grass_spread.Apply ();




	

		//




		finalTexture = new Texture2D(width, height);
		for (int x = 0; x < finalTexture.width; x++) {
			for (int y = 0; y < finalTexture.height; y++) {	
				Color color = Color.magenta;
				float step = 1.0f / 7;
				float h = map [x, y];
				if (h > 6 * step) {

//					color = Color.white;
					Texture2D temp = snow_spread;
					color = temp.GetPixel(x,y);


				} else if (h > 5 * step) {

					float min = 5 * step;
					float max = 6 * step;

					Texture2D temp = rock_spread;
					Color col1 = temp.GetPixel(x,y);
					temp = snow_spread;
					Color col2 = temp.GetPixel(x,y);

//					color = Color.Lerp(Color.grey, Color.white, Normalize(h,min,max));
					color = Color.Lerp(col1, col2, Normalize(h,min,max));
				} else if (h > 4 * step) {
//					color = Color.grey;
					Texture2D temp = rock_spread;
					color = temp.GetPixel(x,y);
				} else if (h > 3 * step) {

					float min = 3 * step;
					float max = 4 * step;

					Texture2D temp = grass_spread;
					Color col1 = temp.GetPixel((x),y);
					temp = rock_spread;
					Color col2 = temp.GetPixel(x,y);

					color = Color.Lerp(col1, col2, Normalize(h,min,max));
//					color = Color.Lerp(grass, rock, Normalize(h,min,max));
				} else if (h > 2 * step) {
//					color = Color.green;
					Texture2D temp = grass_spread;
					color = temp.GetPixel((x),(y));
				} else if (h > 1 * step) {

					float min = 1 * step;
					float max = 2 * step;

					Texture2D temp = water_spread;
					Color col1 = temp.GetPixel(x,y);
					temp = grass_spread;
					Color col2 = temp.GetPixel(x,y);

					color = Color.Lerp(col1, col2, Normalize(h,min,max));
//					color = Color.Lerp(water, grass, Normalize(h,min,max));

				} else {
					color = Color.blue;
					Texture2D temp = water_spread;
					color = temp.GetPixel(x,y);
					//color = Color.Lerp(col1, col2, Normalize(h,min,max));

				}


				finalTexture.SetPixel(x, y, color);
			}
		}
		heatMapTexture.Apply();
		finalTexture.Apply ();

		Material mat = GetComponent<MeshRenderer> ().sharedMaterial;

		mat.SetTexture ("_MainTex", grass_spread);
//		mat.SetTexture ("_ParallaxMap", heatMapTexture);

	}
	//	void RandomFillMap ()
	//	{
	//		if (useRandomSeed) {
	//			seed = Time.time.ToString ();
	//		}
	//
	//		System.Random pseudoRandom = new System.Random (seed.GetHashCode ());
	//
	//		for (int x = 0; x < width; x++) {
	//			for (int y = 0; y < height; y++) {
	//				if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
	//					map [x, y] = 1;
	//				} else {
	//					map [x, y] = (pseudoRandom.Next (0, 100) < randomFillPercent) ? 1 : 0;
	//				}
	//			}
	//		}
	//	}
	//	legacy code
	//	void SmoothMap ()
	//	{
	//		for (int x = 0; x < width; x++) {
	//			for (int y = 0; y < height; y++) {
	//				int neighbourWallTiles = GetSurroundingWallCount (x, y);
	//
	//				if (neighbourWallTiles > 4)
	//					map [x, y] = 1;
	//				else if (neighbourWallTiles < 4)
	//					map [x, y] = 0;
	//
	//			}
	//		}
	//	}
	// 	legacy code
	//	int GetSurroundingWallCount (int gridX, int gridY)
	//	{
	//		int wallCount = 0;
	//		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
	//			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {
	//				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
	//					if (neighbourX != gridX || neighbourY != gridY) {
	//						wallCount += map [neighbourX, neighbourY];
	//					}
	//				} else {
	//					wallCount++;
	//				}
	//			}
	//		}
	//
	//		return wallCount;
	//	}
	//


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


	float Normalize (float val, float min, float max){
		return (val - min) / (max - min);
	}

}