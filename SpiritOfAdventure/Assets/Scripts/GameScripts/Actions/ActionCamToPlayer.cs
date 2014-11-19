using UnityEngine;
using System.Collections;

public class ActionCamToPlayer : Action
{
	public float leadingDistance = 6f;
	public float timeToReachTarget = 0.5f;

	private CameraMan cameraMan;
	private GameObject player;

	void Awake()
	{
		cameraMan = FindObjectOfType<CameraMan>() as CameraMan;
		player = FindObjectOfType<Player>().gameObject;
	}

	protected override void Activate()
	{
		cameraMan.FollowObject(player);
		cameraMan.leadingDistance = 8f;
		cameraMan.timeToReachTarget = 1f;
		ActionComplete();
	}
}