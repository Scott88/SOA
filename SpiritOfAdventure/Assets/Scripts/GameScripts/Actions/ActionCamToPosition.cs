using UnityEngine;
using System.Collections;

public class ActionCamToPosition : SOAAction
{
	public float leadingDistance = 10f;
	public float timeToReachTarget = 1f;

	public Vector3 worldPosition;

	private CameraMan cameraMan;

	void Awake()
	{
		cameraMan = FindObjectOfType<CameraMan>() as CameraMan;
	}

	protected override void Activate()
	{
		cameraMan.FollowPosition(worldPosition);
		cameraMan.leadingDistance = leadingDistance;
		cameraMan.timeToReachTarget = timeToReachTarget;
		ActionComplete();
	}
}
