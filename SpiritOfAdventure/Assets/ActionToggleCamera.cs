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
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	protected override void Activate()
	{
	
	}
}
