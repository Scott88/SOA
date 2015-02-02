//~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*
//GameManager.cs
//
//By: Nicholas MacDonald
//Handles the dynamics between mouse down events, spirits, and set pieces
//~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*

using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    

	public GameObject greenSpiritPref, redSpiritPref, blueSpiritPref;
	public Vector2 spiritSpawnOffset;
	public SpiritType currentSpirit { get; set; }
	public EffectManager effectManager { get; set; }
	public HintManager hintManager { get; set; }
	public ScoreManager scoreManager { get; set; }

	private Spirit greenSpirit, redSpirit, blueSpirit;
	private SpiritGUI greenGUI, redGUI, blueGUI;
	private bool spiritHeld;
	private bool mouseHeldOnGUI;
	private Camera spiritGUICamera;
	private bool spiritGUIClicked;
	private MonsterSpawner activeSpawner = null;
	private Collider2D clickedOn;
	

	void Start()
	{
		currentSpirit = SpiritType.ST_NULL;
		spiritGUIClicked = false;
		spiritHeld = false;
		mouseHeldOnGUI = false;

		//We load in each of the object references so that requests for them can funnel in through
		// the manager instead of having everyone trying to get everything.
		effectManager = FindObjectOfType(typeof(EffectManager)) as EffectManager;
		hintManager = FindObjectOfType(typeof(HintManager)) as HintManager;
		scoreManager = FindObjectOfType(typeof(ScoreManager)) as ScoreManager;
		LevelTimer timer = FindObjectOfType<LevelTimer>() as LevelTimer;
		timer.guiText.enabled = true;

		//We get our spirit buttons and the camera for the raycasting
		greenGUI = GameObject.Find("SpiritGUICamera/GUIContainer/Green Spirit Button").GetComponent<SpiritGUI>();
		redGUI = GameObject.Find("SpiritGUICamera/GUIContainer/Red Spirit Button").GetComponent<SpiritGUI>();
		blueGUI = GameObject.Find("SpiritGUICamera/GUIContainer/Blue Spirit Button").GetComponent<SpiritGUI>();
		spiritGUICamera = GameObject.Find("SpiritGUICamera").GetComponent<Camera>();

        PlayerSpawn[] spawns = FindObjectsOfType<PlayerSpawn>() as PlayerSpawn[];

        int currentPath = SaveFile.Instance().GetPath(Application.loadedLevelName);

        Player player = FindObjectOfType<Player>() as Player;

        bool playerSpawned = false;

        for (int j = 0; j < spawns.Length && !playerSpawned; j++)
        {
            if (spawns[j].pathNumber == currentPath)
            {
                player.transform.position = spawns[j].transform.position;
                playerSpawned = true;
            }
        }

        if (!playerSpawned)
        {
            player.transform.position = spawns[0].transform.position;
        }

        CameraMan cameraMan = FindObjectOfType<CameraMan>() as CameraMan;

        cameraMan.FollowObject(player.gameObject);

		//We instantiate each of our spirits from the game manager.
		GameObject greenSpiritObject = (GameObject)Instantiate(greenSpiritPref, player.transform.position, transform.rotation);

		greenSpirit = greenSpiritObject.GetComponent<Spirit>();
		greenSpirit.SetManager(this);

        GameObject redSpiritObject = (GameObject)Instantiate(redSpiritPref, player.transform.position, transform.rotation);

		redSpirit = redSpiritObject.GetComponent<Spirit>();
		redSpirit.SetManager(this);

        GameObject blueSpiritObject = (GameObject)Instantiate(blueSpiritPref, player.transform.position, transform.rotation);

		blueSpirit = blueSpiritObject.GetComponent<Spirit>();
		blueSpirit.SetManager(this);

        greenSpiritObject.GetComponent<FollowPlayer>().SetPlayer(player.gameObject);
        redSpiritObject.GetComponent<FollowPlayer>().SetPlayer(player.gameObject);
        blueSpiritObject.GetComponent<FollowPlayer>().SetPlayer(player.gameObject);

		//To be completely honest, I'm not 100% sure why I'm spawning the spirits from the gameManager when they would still work
		// being in the scene. It might still be a holdover from one of the old ways spirits worked, but this way, that at least
		// always spawn relative to the player.
	}

	//In the LateUpdate, we check for mouse events and interpret any that occur.
	void LateUpdate()
	{
		if (PauseButton.paused == true || GameWin.Win)
		{
			return;
		}

		if(Input.GetKeyDown(KeyCode.O))
		{
			Time.timeScale = 3f;
		}

		if (Input.GetKeyDown(KeyCode.P))
		{
			Time.timeScale = 1f;
		}

		if (Input.GetMouseButtonDown(0))
		{
			GetClickedOn();
		}
		else if(Input.GetMouseButton(0))
		{
			GetHeldOn();
		}


		//If left mouse was pushed this frame
		if(Input.GetMouseButtonUp(0))
		{
			//We get the object that was clicked on, if any.
			GetReleasedOn();

			//If a spirit is currently selected, but literally nothing was clicked on, then we send the spirit to the
			// position of the mouse click to fizzle.
			if(currentSpirit != SpiritType.ST_NULL && !spiritGUIClicked)
			{
				if(currentSpirit == SpiritType.ST_GREEN)
				{
					SpiritFailed(greenGUI, greenSpirit);
				}
				if(currentSpirit == SpiritType.ST_RED)
				{
					SpiritFailed(redGUI, redSpirit);
				}
				if(currentSpirit == SpiritType.ST_BLUE)
				{
					SpiritFailed(blueGUI, blueSpirit);
				}

				currentSpirit = SpiritType.ST_NULL;
			}
			//else, we just assume we clicked on the spiritGUI this frame and then acknowledge that we're done
			// caring about the fact that we clicked on it.
			else
			{
				spiritGUIClicked = false;
			}
		}
	}

	//Convenience method that resets some of the static variables in the game for the start of a new level.
	public static void ResetGame()
	{
		GameWin.Win = false;
		GameWin.goingToWin = false;

		//PauseButton.paused = false;

		Time.timeScale = 1f;
	}

	//Probably my favorite method name in the whole project so far, this method gets the object that we clicked on
	// this frame.
	void GetClickedOn()
	{
		//We start with the spiritGUI as it has the highest priority so we raycast from the spiritGUI camera to the
		// GUI objects.
		Vector2 rayOrigin = (Vector2)(spiritGUICamera.ScreenToWorldPoint(Input.mousePosition));
		Vector2 rayDirection = new Vector2();

		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, 100, 1 << 12);

		//If any of them are hit, get their type and select that spirit if the button isn't on cooldown.
		if (hit)
		{
			SpiritGUI tempGUI = hit.collider.GetComponent<SpiritGUI>();

			if (!tempGUI.IsOnCooldown())
			{
				clickedOn = hit.collider;
			}
			else
			{
				tempGUI.audio.Play();
			}

			mouseHeldOnGUI = true;

			//We also get out early if we find which object we clicked on early!
			return;
		}

		rayOrigin = (Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		hit = Physics2D.Raycast(rayOrigin, rayDirection, 100, 1 << 18);

		if (hit)
		{
			clickedOn = hit.collider;

			mouseHeldOnGUI = false;
		}
	}

	void GetHeldOn()
	{
		if (clickedOn && !spiritHeld)
		{
			if (mouseHeldOnGUI)
			{
				//We start with the spiritGUI as it has the highest priority so we raycast from the spiritGUI camera to the
				// GUI objects.
				Vector2 rayOrigin = (Vector2)(spiritGUICamera.ScreenToWorldPoint(Input.mousePosition));
				Vector2 rayDirection = new Vector2();

				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, 100, 1 << 12);

				//If any of them are hit, get their type and select that spirit if the button isn't on cooldown.
				if (!hit || hit.collider != clickedOn)
				{
					spiritHeld = true;
					SpiritGUI tempGUI = clickedOn.GetComponent<SpiritGUI>();
					SelectSpirit(tempGUI.type);

					//We also get out early if we find which object we clicked on early!
					return;
				}
			}
			else
			{
				Vector2 rayOrigin = (Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition));
				Vector2 rayDirection = new Vector2();

				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, 100, 1 << 18);

				if (!hit)
				{
					spiritHeld = true;
				}
			}
		}
		else if (spiritHeld)
		{
			if (currentSpirit == SpiritType.ST_GREEN)
			{
				greenSpirit.MoveHere(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			}
			else if (currentSpirit == SpiritType.ST_RED)
			{
				redSpirit.MoveHere(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			}
			else if (currentSpirit == SpiritType.ST_BLUE)
			{
				blueSpirit.MoveHere(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			}
		}
	}

	void GetReleasedOn()
	{
		Vector2 rayOrigin = new Vector2();
		Vector2 rayDirection = new Vector2();
		RaycastHit2D hit = new RaycastHit2D();

		if (!spiritHeld)
		{
			//We start with the spiritGUI as it has the highest priority so we raycast from the spiritGUI camera to the
			// GUI objects.
			rayOrigin = (Vector2)(spiritGUICamera.ScreenToWorldPoint(Input.mousePosition));
			rayDirection = new Vector2();

			hit = Physics2D.Raycast(rayOrigin, rayDirection, 100, 1 << 12);

			//If any of them are hit, get their type and select that spirit if the button isn't on cooldown.
			if (hit)
			{
				if (hit.collider == clickedOn)
				{
					SpiritGUI tempGUI = hit.collider.GetComponent<SpiritGUI>();

					if (!tempGUI.IsOnCooldown())
					{
						spiritGUIClicked = true;
						SelectSpirit(tempGUI.type);
					}
					else
					{
						tempGUI.audio.Play();
					}

					clickedOn = null;
					spiritHeld = false;

					//We also get out early if we find which object we clicked on early!
					return;
				}
			}
		}

		//If no spirit button was clicked, then we check the normal camera to see if we clicked on
		// any set pieces, first checking essential puzzle pieces and then checking ordinary set pieces.
		rayOrigin = (Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		hit = Physics2D.Raycast(rayOrigin, rayDirection, 100, 1 << 14);

		if (hit)
		{
			SpiritReceiver tempReceiver = hit.collider.GetComponent<SpiritReceiver>();
			SendSpirit(tempReceiver);

			clickedOn = null;
			spiritHeld = false;

			return;
		}

		hit = Physics2D.Raycast(rayOrigin, rayDirection, 100, 1 << 15);

		if (hit)
		{
			SpiritReceiver tempReceiver = hit.collider.GetComponent<SpiritReceiver>();
			SendSpirit(tempReceiver);
		}

		clickedOn = null;
		spiritHeld = false;
	}

	//If the spirit failed, we put it on cooldown and tell the spirit to go fizzle in that area
	void SpiritFailed(SpiritGUI gui, Spirit spirit)
	{
		gui.PutOnCooldown();

		Vector3 tempVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		tempVec.z = 0f;

		spirit.MoveHereAndPoof(tempVec);
	}

	//If it succeeded, we give the spirit the set piece and tell it to trigger it when it reaches it
	void SpiritSucceeded(SpiritGUI gui, Spirit spirit, SpiritReceiver receiver)
	{
		gui.PutOnCooldown();

		Vector3 tempVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		tempVec.z = 0f;

		spirit.MoveHereAndTrigger(tempVec, receiver);
	}

	//Holdover helper method for setting the last set piece clicked
	public void SendSpirit(SpiritReceiver receiver)
	{
		//If our current selected spirit is green
		if (currentSpirit == SpiritType.ST_GREEN)
		{
			//If if the set piece accepts green, we send the spirit there to trigger the SpiritReceiver.
			//If not than just like above, we send the spirit to fizzle
			if (currentSpirit == receiver.compatableType)
			{
				SpiritSucceeded(greenGUI, greenSpirit, receiver);
			}
			else
			{
				SpiritFailed(greenGUI, greenSpirit);
			}
		}
		if (currentSpirit == SpiritType.ST_RED)
		{
			if (currentSpirit == receiver.compatableType)
			{
				SpiritSucceeded(redGUI, redSpirit, receiver);
			}
			else
			{
				SpiritFailed(redGUI, redSpirit);
			}
		}
		if (currentSpirit == SpiritType.ST_BLUE)
		{
			if (currentSpirit == receiver.compatableType)
			{
				SpiritSucceeded(blueGUI, blueSpirit, receiver);
			}
			else
			{
				SpiritFailed(blueGUI, blueSpirit);
			}
		}

		currentSpirit = SpiritType.ST_NULL;

	}

	//Helper method for selecting spirits and deselecting current spirits if one
	// is already selected
	public void SelectSpirit(SpiritType type)
	{
		if(currentSpirit != type)
		{
			if (currentSpirit == SpiritType.ST_GREEN)
			{
				greenSpirit.Deselect();
			}
			else if (currentSpirit == SpiritType.ST_RED)
			{
				redSpirit.Deselect();
			}
			else if (currentSpirit == SpiritType.ST_BLUE)
			{
				blueSpirit.Deselect();
			}

			if (type == SpiritType.ST_GREEN)
			{
				greenSpirit.Select();
				currentSpirit = type;
			}
			else if (type == SpiritType.ST_RED)
			{
				redSpirit.Select();
				currentSpirit = type;
			}
			else if (type == SpiritType.ST_BLUE)
			{
				blueSpirit.Select();
				currentSpirit = type;
			}
		}
	}

	public void SetActiveSpawner(MonsterSpawner spawner)
	{
		activeSpawner = spawner;
	}

	public bool TrySpawnMonster(float timeRemaining)
	{
		if (!activeSpawner.IsVisible())
		{
			activeSpawner.SpawnMonster(timeRemaining);
			return true;
		}

		return false;
	}

	public void ForceSpawnMonster(float timeRemaining)
	{
		activeSpawner.SpawnMonster(timeRemaining);
	}
}
