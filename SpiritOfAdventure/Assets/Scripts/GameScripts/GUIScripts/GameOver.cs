using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

	public GUISkin mySkin;

	void OnGUI () {

		GUI.skin = mySkin;
		// Make a background box
		//GUI.Box(new Rect(10,10,100,90), "Main Menu");
		
		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if(GUI.Button(new Rect(Screen.width * (1f/2.5f),Screen.height * (1f/2.2f),Screen.width * (1f/6f), Screen.height * (1f/8f)), "Main Menu"))
		{
			Application.LoadLevel("MainMenu");
			Time.timeScale = 1;
		}

	}
}