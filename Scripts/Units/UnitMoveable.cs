﻿using UnityEngine;
using System.Collections;

public class UnitMoveable : MonoBehaviour {


	private bool isMoving = false;
	private Vector3 vDestination;
	private Animator animator;

	public float movementSpeed = 5.0f;
	public float destinationTolerance = 1f;
	// Use this for initialization
	void Start () {
		isMoving = false;
		animator = GetComponent<Animator> ();
	}

	private float dist, time;
	// Update is called once per frame
	void Update () {
		if (isMoving) {
			vDestination.y = transform.position.y;
			if (Vector3.Distance(transform.position, vDestination) <= destinationTolerance) {
				isMoving = false;
				animator.SetFloat ("Speed", 0);
			} else {
				float distCovered = (Time.time - time) * movementSpeed;
				float fracJourney = distCovered / dist;
				Vector3 v = Vector3.Lerp (transform.position, vDestination, fracJourney);
				RaycastHit hit;
				if (Physics.Raycast (v, Vector3.down, out hit, 5.0f)) {
					v.y = hit.point.y + GetComponent<Collider>().bounds.extents.y;
				}
				animator.SetFloat ("Speed", movementSpeed);
				transform.position = v;
			}
		}
	}

	public void MoveTo(Vector3 vPoint) {
		transform.LookAt (new Vector3(vPoint.x, transform.position.y, vPoint.z));
		GetComponent<UnitBehaviour> ().resetCircle ();
		vDestination = vPoint;
		time = Time.time;
		dist = Vector3.Distance (transform.position, vPoint);
		isMoving = true;
	}
}
