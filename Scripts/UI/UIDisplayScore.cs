using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
public class UIDisplayScore : MonoBehaviour {
	Text txt;

	int counter=0;
	public Texture2D btnTexture;

	// Use this for initialization
	void Start () {
		this.tag="ui";
		txt = GetComponent<Text> ();
		displayScore (0,0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void displayScore(int min, int max){
		txt.text = "Captured territories: " +min+ "/" + max ;
	}

	void OnGUI() {



		List<string> msgs = new List<string>();
		msgs.Add ("Game objectives \n" +
		"Capture all territories\n" +
		"\tYou can capture territories by walking on it \n when it is not occupied by other player");
		msgs.Add ("Units \n");

		msgs.Add ("Unit controll \n"+
			"You can select and move units with your mouse\n"+
			"You can create new unit by right clicking on the existing one.");

		msgs.Add ( "Combat system \n"+
			"Units will automatically attack the closest enemy in range");



		if(counter<msgs.Count){
			GUI.Box(new Rect(Screen.width/4,40,Screen.width/2,Screen.height/2),"Tutorial:\n" + msgs[counter]);

			if (GUI.Button (new Rect (Screen.width/2-25, Screen.height/2+10, 50, 30), "Next")) {
				counter++;
			}
		}
			





	}

}
