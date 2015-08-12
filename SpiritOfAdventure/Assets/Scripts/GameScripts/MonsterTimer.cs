using UnityEngine;
using System.Collections;

public class MonsterTimer : MonoBehaviour
{
	public GameObject tagMonsterPref;

	private float originalDistance;
	private float timeToReach = 10f, timer;
	private Player player;
	private SpringJoint2D joint;
	private LevelTimer levelTimer;

	void Awake()
	{
		joint = GetComponent<SpringJoint2D>();
		player = FindObjectOfType(typeof(Player)) as Player;
		levelTimer = FindObjectOfType(typeof(LevelTimer)) as LevelTimer;
	}

	void Start()
	{
		joint.connectedBody = player.GetComponent<Rigidbody2D>();
		joint.distance = originalDistance = Vector3.Distance(transform.position, player.transform.position);
		timer = 0f;
	}

	void Update()
	{
		if(!GameWin.HasWon())
		{
			timer += Time.deltaTime;

			if (timer >= timeToReach)
			{
				Player.Stop();
				Instantiate(tagMonsterPref, transform.position, Quaternion.identity);
				levelTimer.enabled = false;
				Destroy(gameObject);
			}

			joint.distance = Mathf.Lerp(originalDistance, 2.48f, timer / timeToReach);
		}
		else if (joint.enabled)
		{
			joint.enabled = false;
		}
	}

	public void SetTimeLeft(float timeLeft)
	{
		timeToReach = timeLeft;
	}
}
