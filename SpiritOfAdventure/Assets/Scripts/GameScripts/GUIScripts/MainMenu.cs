using UnityEngine;
using System.Collections;

public class MainMenu : FocusPoint
{
	public GUISkin mySkin;
	public FocusPoint firstChapter, credits;

	private MenuFocus focus;

	void Start()
	{
		focus = FindObjectOfType<MenuFocus>() as MenuFocus;

		if (gameObject.name != LevelQueue.menuContext)
		{
			gameObject.SetActive(false);
		}
	}

	void OnGUI () 
	{
		GUI.skin = mySkin;

		GUI.Label(new Rect(Screen.width * (0.29f), Screen.height * (0.23f), Screen.width * (0.4f), Screen.height * (0.16f)), "Spirit Of Adventure");

		if(GUI.Button(new Rect(Screen.width * (1f/2.5f),Screen.height * (0.4f),Screen.width * (1f/6f), Screen.height * (1f/8f)), "Start"))
		{
			focus.PlayButtonSound();
			focus.SetTarget(firstChapter);
		}

		if(GUI.Button(new Rect(Screen.width * (1f/2.5f),Screen.height * (0.55f),Screen.width * (1f/6f), Screen.height * (1f/8f)), "Instructions")) 
		{
			focus.PlayButtonSound();
			focus.SetTarget(credits);
			//PlayerPrefs.DeleteAll();
		}
	}
}
