using UnityEngine;
using System.Collections;

public class ActionStopPlayer : Action
{
	public bool attachPlayer;

	protected override void Activate()
	{
		ActionComplete();

		Player.Stop();
	}
}