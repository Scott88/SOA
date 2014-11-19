using UnityEngine;
using System.Collections;

public class ActionToggleCollider : Action
{
	public bool startEnabled;
	
	void Start()
	{
		collider2D.enabled = startEnabled;
	}

	protected override void Activate()
	{
		collider2D.enabled = !collider2D.enabled;
		ActionComplete ();
	}
	
}