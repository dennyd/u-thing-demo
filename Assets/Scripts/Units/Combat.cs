using UnityEngine;
using System.Collections;

public class Combat : MonoBehaviour {


	public GameObject target;

	public float baseHealth = 100;
	public float health = 0;

	public float baseAttack = 15;
	public float attack = 0;


	private float lastShot, lastShotTaken;
	private Animator animator;

	void Start() {
		if (health == 0) health = baseHealth;
		if (attack == 0) attack = baseAttack;

		lastShot = Time.time;
		animator = GetComponent<Animator> ();

		updateHealth ();
	}

	// Update is called once per frame
	void Update () {
		if (isAlive() && target != null) {
			transform.LookAt (target.transform.position);
			GetComponent<Animator>().SetBool("HasTarget", true);

			float deltaTime = Time.time - lastShot;
			if (deltaTime > 2f) {
				shoot ();
				lastShot = Time.time;
			}
		}
		if (!isAlive() && lastShotTaken != null && (Time.time - lastShotTaken) > 5) {
			Destroy (gameObject);
		}
	}

	void shoot() {
		if (target.GetComponent<Combat> ().isAlive ()) {
			try {
				StartCoroutine (shootAnim ());
				target.GetComponent<Combat> ().RemoveHealth (attack);
			} catch {
				Debug.Log ("Cannot animate shoot");
			}
		} else {
			target = null;
			animator.SetBool ("HasTarget", false);
		}
	}

	IEnumerator shootAnim() {
		animator.SetBool("Shooting", true);
		yield return new WaitForSeconds(0.1f);
		animator.SetBool("Shooting", false);
		yield return new WaitForSeconds(0.2f);
		target.GetComponent<Animator>().SetBool("Hit", true);
		yield return new WaitForSeconds(0.1f);
		target.GetComponent<Animator>().SetBool("Hit", false);
	}

	public void SetTarget(GameObject target) {
		if (target == gameObject)
			this.target = null;
		else this.target = target;
	}

	public void RemoveHealth(float val) {
		health -= val;
		lastShotTaken = Time.time;
		if (!isAlive ()) {
			try {
				FindObjectOfType<MovementController> ().deactivateUnit (gameObject);
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
			} catch {
				Debug.Log ("Cannot unregister");
			}
		}
		updateHealth ();
	}

	public bool isAlive() {
		return health > 0;
	}

	private void updateHealth() {
		try {
			animator.SetFloat("Health", health);
		} catch {
			Debug.Log ("Cannot set health in animator");
		}
	}
}
