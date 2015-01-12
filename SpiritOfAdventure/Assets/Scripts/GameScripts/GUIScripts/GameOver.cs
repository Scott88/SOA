using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

	public GUISkin mySkin;

	void Start()
	{
		if (Screen.width >= (1080)) 
		{
			mySkin.button.fontSize = Screen.width / (40);
			mySkin.label.fontSize = Screen.width / (20);
		}
		
		if (Screen.width < (1080))
		{
			mySkin.button.fontSize = Screen.width / (30);
			mySkin.label.fontSize = Screen.width / (20);
		}
	}

	void OnGUI () {

		GUI.skin = mySkin;
		// Make a background box
		//GUI.Box(new Rect(10,10,100,90), "Main Menu");
		
		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if(GUI.Button(new Rect(Screen.width * (0.36f),Screen.height * (0.32f),Screen.width * (0.25f), Screen.height * (1f/7f)), "Menu"))
		{
			Application.LoadLevel("MainMenu");
			Time.timeScale = 1;
		}

	}
}