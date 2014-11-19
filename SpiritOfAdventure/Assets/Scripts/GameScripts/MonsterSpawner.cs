using UnityEngine;
using System.Collections;

public class MonsterSpawner : MonoBehaviour
{
	public GameObject monsterPrefab;
	public bool defaultSpawner;
	public Vector3 spawnOffset;

	private GameManager gameManager;

	void Awake()
	{
		gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
	}

	void Start()
	{
		if (defaultSpawner)
		{
			gameManager.SetActiveSpawner(this);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			gameManager.SetActiveSpawner(this);
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawIcon(transform.position, "MonsterSpawner.png", false);
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.black;

		Gizmos.DrawSphere(transform.position + spawnOffset, 0.2f);
	}

	public bool IsVisible()
	{
		return (transform.position + spawnOffset).IsVisibleFrom(Camera.main);
	}

	public void SpawnMonster(float timeRemaining)
	{
		MonsterTimer evilMonster = ((GameObject)Instantiate(monsterPrefab, transform.position + spawnOffset, Quaternion.identity)).GetComponent<MonsterTimer>();
		evilMonster.SetTimeLeft(timeRemaining);
	}
}