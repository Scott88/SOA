using UnityEngine;
using System.Collections;

public class ActionPlayParticle : SOAAction
{
	protected override void Activate()
	{
		ActionComplete();
		particleSystem.Play();
	}
}