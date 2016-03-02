using UnityEngine;
using System.Collections;

[System.Serializable]
public class ColorScheme {
	public float R, G, B;
	public ColorScheme(float r, float g, float b) {
		this.R = r;
		this.G = g;
		this.B = b;
	}
	public Color getColor() {
		return new Color (R / 256, G / 256, B / 256, 1.0f);
	}
	public ColorScheme(ColorScheme a, ColorScheme b) {
		this.R = (a.R + b.R) / 2;
		this.G = (a.G + b.G) / 2;
		this.B = (a.B + b.B) / 2;
	}
}

public class moveTest : MonoBehaviour {

	private static moveTest Selected;
	private static ColorScheme defaultColorScheme = new ColorScheme(100, 100, 100), selectedColorScheme = new ColorScheme(256, 256, 256);

	public float movementMultiplier, movementSpeed;

	private Rigidbody rb;
	private Animator animator;
	private Renderer rend;

	public ColorScheme colorscheme = moveTest.defaultColorScheme, activeColorScheme = moveTest.selectedColorScheme;

	private ColorScheme fireColorScheme = new ColorScheme(256, 0, 0), waterColorScheme = new ColorScheme(0, 0, 256), grassColorScheme = new ColorScheme(0, 256, 0);

	void Start() {
		rb = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		rend = transform.Find("Body").GetComponent<Renderer>();
	}

	void OnMouseDown() {
		if (moveTest.Selected == this) {
			moveTest clone = (moveTest) Instantiate (this, new Vector3 (transform.position.x + Random.Range (0, 5), transform.position.y, transform.position.z + Random.Range (0, 5)), transform.rotation);

			Debug.Log (Input.GetKey(KeyCode.LeftControl));
			clone.colorscheme = new ColorScheme (
				(Input.GetKey(KeyCode.LeftControl) ? fireColorScheme : 
					(Input.GetKey(KeyCode.LeftAlt) ? waterColorScheme : grassColorScheme)
				), this.colorscheme);
		}
		moveTest.Selected = this;
	}

	private float speed;
	// Use this for initialization
	void FixedUpdate() {

		if (moveTest.Selected == this) {
			float movementX = Input.GetAxis ("Horizontal");
			float movementZ = Input.GetAxis ("Vertical");
			rb.velocity = new Vector3 (movementX * movementMultiplier, 0.0f, movementZ * movementMultiplier);
			speed = Mathf.Abs ((movementX + movementZ) * movementMultiplier);
		}

		activeColorScheme = (Selected == this) ? selectedColorScheme : colorscheme;
		rend.material.SetColor("_Color", activeColorScheme.getColor());

		// click 
		handleClick();
//		move();

		animator.SetBool ("Selected", moveTest.Selected == this);
		animator.SetFloat ("Speed", speed);
	}

	private Vector3 startCoords, endCoords;
	private bool shouldMove = false; 
	private float dist, time;

	void handleClick() {
		bool lclick = Input.GetMouseButton (0), rclick = Input.GetMouseButton (1);
		if (lclick || rclick) {
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit hit;
			if( Physics.Raycast( ray, out hit, Mathf.Infinity ) )
			{
				if (!System.Text.RegularExpressions.Regex.IsMatch(hit.transform.gameObject.name, "alien", System.Text.RegularExpressions.RegexOptions.IgnoreCase)) {
					if (lclick) {
						moveTest.Selected = null;
					}
					if (rclick) {
						if (Selected == this) {
//							shouldMove = true;
//							startCoords = transform.position; 
//							endCoords = hit.point;
//							dist = Vector3.Distance (startCoords, endCoords);
//							time = Time.time;
//							transform.LookAt (endCoords);
//							speed = movementSpeed * 2;

							UnitMoveable moveScript = GetComponent<UnitMoveable>();
							if (moveScript) {
								moveScript.MoveTo (hit.point);
							}
						}
					}
				}
			}
		}
	}

//	void move() {
//		if (shouldMove) { 
//			Debug.Log ("CLICKS " + startCoords + " " + endCoords);
//			float distCovered = (Time.time - time) * movementSpeed;
//			float fracJourney = distCovered / dist;
//			Vector3 lerp = Vector3.Lerp (startCoords, endCoords, fracJourney);
//			transform.position = lerp;
//			animator.SetFloat ("Speed", movementSpeed);
//			if (Vector3.Distance(endCoords, lerp) == 0) {
//				shouldMove = false;
//				speed = 0;
//			};
//		}
//	}
		
}