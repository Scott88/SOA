using UnityEngine;
using System.Collections;

public class ActionStopTimerEarly : Action
{
	private LevelTimer timer;

	void Awake()
	{
		timer = FindObjectOfType<LevelTimer>() as LevelTimer;
	}

	protected override void Activate()
	{
		timer.gameObject.SetActive(false);
		//GameWin.goingToWin = true;
		ActionComplete();
	}
}