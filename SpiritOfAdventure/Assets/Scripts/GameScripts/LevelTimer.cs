using UnityEngine;
using System.Collections;

public class LevelTimer : MonoBehaviour 
{
	public bool disabled = false;
	public float timer = 60f;
	public float spawnMonsterAt = 10f;
	public float forceMonsterSpawnAt = 5f;

	private float originalLength;
	private string display;
	private bool monsterSpawned = false;
	private GameManager gameManager;
	private bool soundPlayed = false;

	void Awake()
	{
		gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
	}

	void Start()
	{
		originalLength = timer;

		if (disabled)
		{
			gameObject.SetActive(false);
		}
	}

	void Update()
	{
		timer -= Time.deltaTime;
		guiText.text = display = timer.ToString("0");

		if ((GameWin.HasWon() || PauseButton.paused) && guiText.enabled)
		{
			guiText.enabled = false;
		}
		else if (!(GameWin.HasWon() || PauseButton.paused) && !guiText.enabled)
		{
			guiText.enabled = true;
		}

		if (timer <= spawnMonsterAt && !monsterSpawned)
		{
			monsterSpawned = gameManager.TrySpawnMonster(timer);
		}

		if (timer <= forceMonsterSpawnAt && !monsterSpawned)
		{
			gameManager.ForceSpawnMonster(timer);
			monsterSpawned = true;
		}

		if (timer <= 3f && !soundPlayed)
		{
			audio.Play();
			soundPlayed = true;
		}
	}

	public float GetTimeTaken()
	{
		return originalLength - timer;
	}

	public float GetTimeLeft()
	{
		return timer;
	}

	public string TimeTakenAsString()
	{
		return (originalLength - timer).ToString("0");
	}
}

