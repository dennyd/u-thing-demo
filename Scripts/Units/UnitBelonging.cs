using UnityEngine;
using System.Collections;

public class UnitBelonging : MonoBehaviour {

	public int player = 0;
	public int territory = 0;
	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool isMainPlayer() {
		return player == 1;
	}

	public bool isNeutral() {
		return player == 0;
	}

	public bool isSamePlayerAs(UnitBelonging b) {
		return b.player == player;
	}
}
