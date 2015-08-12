using UnityEngine;
using System.Collections;

public class ActionToggleCollider : SOAAction
{
	public bool startEnabled;
	
	void Start()
	{
		GetComponent<Collider2D>().enabled = startEnabled;
	}

	protected override void Activate()
	{
		GetComponent<Collider2D>().enabled = !GetComponent<Collider2D>().enabled;
		ActionComplete ();
	}
	
}