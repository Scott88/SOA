//~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*
//ActionCameraZoom.cs
//
//By: Nicholas MacDonald
//When triggered, zooms the camera in or out to a specified orthographic size
//~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*

using UnityEngine;
using System.Collections;

public class ActionCameraZoom : Action 
{
	public float zoomTo = 5f;
	public float timer = 1f;

	private bool zoom;
	private float currentZoom;
	private float velocityRef;
	
	void FixedUpdate () 
	{
		//If we have been activated.
		if ( zoom && !GameWin.Win)
		{
			//We use the ever so wonderful SmoothDamp method to smoothly transition the camera to its second position
			currentZoom = Mathf.SmoothDamp(currentZoom, zoomTo, ref velocityRef, timer, Mathf.Infinity, Time.fixedDeltaTime);

			Camera.main.orthographicSize = currentZoom;

			//Since SmoothDamp never actually reachs its exact target, we just check if it's close enough.
			if (Mathf.Abs(currentZoom - zoomTo) < 0.01f)
			{
				zoom = false;
				ActionComplete();
			}
		}	
	}

	//Here, activating the object only flags the action to start updating.
	protected override void Activate()
	{
		zoom = true;
		currentZoom = Camera.main.orthographicSize;
	}
}
