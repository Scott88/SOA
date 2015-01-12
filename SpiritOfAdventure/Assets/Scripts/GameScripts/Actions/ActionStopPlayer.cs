using UnityEngine;
using System.Collections;

public class ActionStopPlayer : SOAAction
{
	public bool attachPlayer;

	protected override void Activate()
	{
		ActionComplete();

		Player.Stop();
	}
}