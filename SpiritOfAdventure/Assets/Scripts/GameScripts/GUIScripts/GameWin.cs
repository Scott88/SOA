using UnityEngine;
using System.Collections;

public class GameWin : MonoBehaviour 
{
	public GUISkin mySkin;
	public float transitionDelay = 7f;
	public string levelName;
	public Camera[] camerasToTurnOff;
	public bool canContinue = true;

    public int nextLevelPath;
    public string nextLevelName;

	public Vector3 winScreenPosition;
	public float targetZoom;

	public static Animator animator;
	public static bool Win;
	public static bool goingToWin = false;

	private static bool winTriggered = false;
	private GameManager gameManager;
	private LevelTimer timer;
	private CameraMan cameraMan;
	private Vector3 posRef;
	private float zoomRef;
	private bool cameraManTriggered = false;
	private bool skippedScore = false;


	void Awake()
	{
		cameraMan = FindObjectOfType<CameraMan>() as CameraMan;
		gameManager = FindObjectOfType<GameManager>() as GameManager;
		timer = FindObjectOfType<LevelTimer>() as LevelTimer;
		Win = false;
		animator = GetComponent<Animator>();
		timer.guiText.enabled = true;

	}

	void Start()
	{
		if (Screen.width >= (1080)) 
		{
			mySkin.button.fontSize = Screen.width / (50);
			mySkin.label.fontSize = Screen.width / (40);
		}
		
		if (Screen.width < (1080))
		{
			mySkin.button.fontSize = Screen.width / (50);
			mySkin.label.fontSize = Screen.width / (40);
		}

		winScreenPosition.z = -60f;
		winTriggered = false;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		goingToWin = false;
		winTriggered = true;
		timer.gameObject.SetActive(false);
		Player.VictoryCheer();

        //if (pathA == true) 
        //{
        //    PlayerPrefs.SetInt("pa",1);
        //}

        //if (pathB == true) 
        //{
        //    PlayerPrefs.SetInt("pb",2);
        //    PlayerPrefs.DeleteKey("pa");
        //}

        //if (pathC == true) 
        //{
        //    PlayerPrefs.SetInt("pc",3);
        //}

        SaveFile.Instance().SetLevelPath(nextLevelName, nextLevelPath);
	}

	void Update()
	{
		if (winTriggered && !Win)
		{
			transitionDelay -= Time.deltaTime;
		}

		if (transitionDelay <= 0f && !PauseButton.paused)
		{
			if (!cameraManTriggered)
			{
				cameraMan.FollowPosition(winScreenPosition);
				cameraMan.timeToReachTarget = 0.5f;
				cameraMan.maximumSpeed = 30f;
				cameraManTriggered = true;
			}

			//Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, winScreenPosition, ref posRef, 0.5f, 30f);
			Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, targetZoom, ref zoomRef, 0.5f, 30f);

			if (Vector3.Distance(Camera.main.transform.position, winScreenPosition) < 0.1f && !Win)
			{
				ActiveWinScreen();
			}
		}

		if (Win)
		{
			if (Input.GetMouseButtonDown(0))
			{
				skippedScore = true;
				gameManager.scoreManager.SkipAnimation();
			}
		}
	}

	void ActiveWinScreen()
	{
		Win = true;
		winTriggered = false;

		gameManager.scoreManager.CalculateFinalScore(timer.GetTimeLeft());

		for (int j = 0; j < camerasToTurnOff.Length; j++)
		{
			camerasToTurnOff[j].enabled = false;
		}
	}
	
	void OnGUI() 
	{
		GUI.skin = mySkin;

		if (Win == true)
		{
			if (!gameManager.scoreManager.IsAnimatingScore())
			{
				if (canContinue)
				{
					if (GUI.Button(new Rect(Screen.width * (1f / 3.2f), Screen.height * (0.84f), Screen.width * (1f / 6f), Screen.height * (1f / 8f)), "Continue?"))
					{
						gameManager.effectManager.PlayButtonSound();
						GameManager.ResetGame();

						if (Application.loadedLevel + 1 >= Application.levelCount)
						{
							LevelQueue.LoadLevel("NullLevel");
						}
						else
						{
                            if (nextLevelName == "")
                            {
                                LevelQueue.LoadLevel(Application.loadedLevel + 1);
                            }
                            else
                            {
                                LevelQueue.LoadLevel(nextLevelName);
                            }
						}
					}
				}

				if (GUI.Button(new Rect(Screen.width * (1f / 1.9f), Screen.height * (0.84f), Screen.width * (1f / 6f), Screen.height * (1f / 8f)), "Level Select"))
				{
					gameManager.effectManager.PlayButtonSound();
					Application.LoadLevel("MainMenu");
					Time.timeScale = 1;
				}
			}

			if (!skippedScore)
			{
				GUI.Label(new Rect(Screen.width * (2f / 8.5f), Screen.height * (3f / 5.9f), Screen.width * (5f / 9.5f), Screen.height * (0.15f)),
						  "Points: " + gameManager.scoreManager.GetFinalScore().ToString() + " Time Left: " + gameManager.scoreManager.GetDisplayTimer().ToString());
			}
			else
			{
				GUI.Label(new Rect(Screen.width * (2f / 8.5f), Screen.height * (3f / 5.9f), Screen.width * (5f / 9.5f), Screen.height * (0.15f)),
						  "Points: " + gameManager.scoreManager.GetActualFinalScore().ToString() + " Time Left: 0");
			}

			GUI.Label(new Rect(Screen.width * (2f / 8.5f), Screen.height * (3f / 8.5f), Screen.width * (5f / 9.5f), Screen.height * (0.15f)), levelName);
		}
		
	}

	public static bool HasWon()
	{
		return goingToWin || winTriggered || Win;
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawIcon(transform.position, "WinTrigger.png", false);
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(winScreenPosition, 0.2f);
	}
}