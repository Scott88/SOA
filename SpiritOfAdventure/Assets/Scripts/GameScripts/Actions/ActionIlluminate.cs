using UnityEngine;
using System.Collections;

public class ActionIlluminate : Action
{
	public Light Mlight = null;

	// Use this for initialization
	void Start()
	{
		Mlight.enabled = false;
	}

	protected override void Activate()
	{
		ActionComplete();

		Mlight.enabled = true;
	}

}