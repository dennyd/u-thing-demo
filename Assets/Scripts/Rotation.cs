using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {
	public float speed = 2f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//		transform.Rotate(Vector3.up, speed * Time.deltaTime);
		transform.RotateAround (new Vector3(GetComponent<MapGenerator>().width/2,0,GetComponent<MapGenerator>().height/2), Vector3.up, speed * Time.deltaTime);
//		transform.RotateA
	}
}
