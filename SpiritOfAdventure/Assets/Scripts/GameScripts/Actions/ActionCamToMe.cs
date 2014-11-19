using UnityEngine;
using System.Collections;

public class ActionCamToMe : Action
{
	public float leadingDistance = 10f;
	public float timeToReachTarget = 1f;

	private CameraMan cameraMan;

	void Awake()
	{
		cameraMan = FindObjectOfType<CameraMan>() as CameraMan;
	}

	protected override void Activate()
	{
		cameraMan.FollowObject(gameObject);
		cameraMan.leadingDistance = leadingDistance;
		cameraMan.timeToReachTarget = timeToReachTarget;
		ActionComplete();
	}
}
