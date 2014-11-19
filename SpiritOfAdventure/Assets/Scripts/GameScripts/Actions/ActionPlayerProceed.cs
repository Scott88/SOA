using UnityEngine;
using System.Collections;

public class ActionPlayerProceed : Action
{
	public GameObject stopTrigger;
	public bool stopMonsterTimer;

	private MonsterTimer monsterTimer;

	void Awake()
	{
		monsterTimer = FindObjectOfType(typeof(MonsterTimer)) as MonsterTimer;
	}

	protected override void Activate()
	{
		Player.Proceed();

		ActionComplete();

		if (stopTrigger)
		{
			stopTrigger.SetActive(false);

			Player.Proceed();
		}
	}
}