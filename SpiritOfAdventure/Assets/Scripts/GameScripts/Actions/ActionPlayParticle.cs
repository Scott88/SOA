using UnityEngine;
using System.Collections;

public class ActionPlayParticle : Action
{
	protected override void Activate()
	{
		ActionComplete();
		particleSystem.Play();
	}
}