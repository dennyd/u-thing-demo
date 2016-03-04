using UnityEngine;
using System.Collections;

public class Combat : MonoBehaviour {


	public GameObject target;

	public float baseHealth = 100;
	public float health = 0;

	public float baseAttack = 15;
	public float attack = 0;

	public float targetRange = 10.0f;

	private float lastShot, lastShotTaken;
	private Animator animator;
	private UnitBelonging _gameData;
	private Projector _p;
	private Texture2D _tex;

	public float healthPercent;
	void Start() {
		if (health == 0) health = baseHealth;
		if (attack == 0) attack = baseAttack;

		lastShot = Time.time;
		animator = GetComponent<Animator> ();

		_gameData = GetComponent <UnitBelonging> ();

		updateHealth ();
	}

	// Update is called once per frame
	void Update () {

		try {

			_p = GetComponent<UnitBehaviour> ().selectionCircle.GetComponent<Projector>();
			_tex = Instantiate( _p.material.GetTexture ("_ShadowTex")) as Texture2D;
//			Texture2D original = _p.material.GetTexture ("_ShadowTex") as Texture2D;
//			_tex = new Texture2D(original.width, original.height);
//			_tex.SetPixels(original.GetPixels());
			_p.material.SetTexture("_ShadowTex", _tex);
		} catch {
			Debug.LogWarning ("Projector not yet ready");
		}

		if (isAlive() && target != null) {
			if (Vector3.Distance (transform.position, target.transform.position) < targetRange) {
				transform.LookAt (target.transform.position);
				GetComponent<UnitBehaviour> ().resetCircle ();
				GetComponent<Animator> ().SetBool ("HasTarget", true);

				float deltaTime = Time.time - lastShot;
				if (deltaTime > 2f) {
					shoot ();
					lastShot = Time.time;
				}
			} else {
				GetComponent<Animator> ().SetBool ("HasTarget", false);
			}
		}
		if (!isAlive() && lastShotTaken != null && (Time.time - lastShotTaken) > 5) {
			Destroy (gameObject);
		}
	}

	void generateTexture() {
		
		Color whiteTransparent = Color.white;
		whiteTransparent.a = 0.0f;

		if (_p != null && _tex != null) {
			
			for (int i = 0; i < _tex.width; i++) {
				for (int j = 0; j < _tex.height; j++) {
					
					if (getAngle(new Vector2(_tex.width / 2, _tex.height / 2), new Vector2(i, j)) / 360 <  healthPercent) {
						_tex.SetPixel(i, j, whiteTransparent);
					} else _tex.SetPixel (i, j, _tex.GetPixel (i, j));

				}
			}
			_tex.Apply ();
		}
		
	}

	private float getAngle(Vector2 fromVector2, Vector2 toVector2) {

		return 180 + Mathf.Atan2(fromVector2.y - toVector2.y, fromVector2.x - toVector2.x) * 180 / Mathf.PI;
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
		if (target != null && !target.GetComponent<UnitBelonging> ().isSamePlayerAs(_gameData)) {
			
			if (target == gameObject)
				this.target = null;
			else
				if (Vector3.Distance(transform.position, target.transform.position) < targetRange)
					this.target = target;

			animator.SetBool ("HasTarget", this.target != null);
		}
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
			healthPercent = 1 - health / baseHealth;
			generateTexture ();
			animator.SetFloat("Health", health);
		} catch {
			Debug.Log ("Cannot set health in animator");
		}
	}
}
