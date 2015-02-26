using UnityEngine;
using System.Collections;

public class PauseButton : MonoBehaviour
{
	public static bool paused;
	public static bool mute;

	public Camera[] camerasToTurnOff;
	public Camera pauseCamera;
	public Camera CameraBGF;
	public Camera camera;
	public GUISkin mySkin;

	private GameManager gameManager;

	void Awake()
	{
		gameManager = FindObjectOfType<GameManager>() as GameManager;

		paused = false;
		mute = false;
		pauseCamera.enabled = false;
		GameWin.Win = false;
	}

	void OnGUI() 
	{
		GUI.skin = mySkin;

		if (paused == false & GameWin.Win == false)
		{	                           //left or right              //up or down				//width						//length
			//GUI.Box(new Rect(Screen.width * (1f/1.15f),Screen.height * (0.1f/6.55f),Screen.width * (0.12f/1f), Screen.height * (0.85f/6.3f)), "Pause Game");
			if (GUI.Button(new Rect(Screen.width * (1f / 60f), Screen.height * (0.1f / 6.55f), Screen.width * (1f / 6f), Screen.height * (1f / 8f)), "Pause"))
			{
                Screen.sleepTimeout = SleepTimeout.SystemSetting;

				gameManager.effectManager.PlayButtonSound();
				Time.timeScale = 0;

				for (int j = 0; j < camerasToTurnOff.Length; j++)
				{
					camerasToTurnOff[j].enabled = false;
				}

				CameraBGF.enabled = false;
				pauseCamera.enabled = true;

				AudioListener.volume = 0f;

				paused = true;

				//renderer.enabled = true;
			}
		}

		if (paused == true & GameWin.Win == false)
		{
			//GUI.Box(new Rect(200,10,100,90), "Pause Menu");
			if (GUI.Button(new Rect(Screen.width * (1f / 2.5f), Screen.height * (1f / 5f), Screen.width * (1f / 6f), Screen.height * (1f / 8f)), "Resume"))
			{
                Screen.sleepTimeout = SleepTimeout.NeverSleep;

				gameManager.effectManager.PlayButtonSound();
				Time.timeScale = 1;

				for (int j = 0; j < camerasToTurnOff.Length; j++)
				{
					camerasToTurnOff[j].enabled = true;
				}

				pauseCamera.enabled = false;
				CameraBGF.enabled = true;
				camera.enabled = true;

				if (!mute)
				{
					AudioListener.volume = 1f;
				}

				paused = false;

			}

			if(GUI.Button(new Rect(Screen.width * (1f/2.5f),Screen.height * (1f/2.2f),Screen.width * (1f/6f), Screen.height * (1f/8f)), "Main Menu"))
			{
				if (!mute)
				{
					AudioListener.volume = 1f;
				}

				gameManager.effectManager.PlayButtonSound();
				GameManager.ResetGame();
				LevelQueue.menuContext = "MainMenu";
				Application.LoadLevel("MainMenu");
				Time.timeScale = 1;
			}

			
			if(GUI.Button(new Rect(Screen.width * (1f/2.5f),Screen.height * (1f/1.4f),Screen.width * (1f/6f), Screen.height * (1f/8f)),"Mute"))
			{
				if (mute)
				{
					//gameManager.effectManager.PlayButtonSound();
					//AudioListener.volume = 1.0f;
					mute = false;
				}
				
				else
				{
					//AudioListener.volume = 0.0f;
					mute = true;
					//PlayerPrefs.DeleteAll();
				}
			}
		}

	}
}