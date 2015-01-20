using UnityEngine;
using System.Collections;

public class ActionToggleCamera  : SOAAction
{
	public Camera[] camerasToTurnOff;
	// Use this for initialization
	void Start ()
	{
		
		for (int j = 0; j < camerasToTurnOff.Length; j++)
		{
			camerasToTurnOff[j].enabled = false;
		}

	
	}

	protected override void Activate()
	{
	
	}
}
