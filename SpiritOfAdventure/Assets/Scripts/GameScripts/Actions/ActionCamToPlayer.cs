using UnityEngine;
using System.Collections;

public class ActionCamToPlayer : SOAAction
{
	public float leadingDistance = 6f;
	public float timeToReachTarget = 0.5f;

	private CameraMan cameraMan;
	private GameObject player;

	void Awake()
	{
		cameraMan = FindObjectOfType<CameraMan>() as CameraMan;
	}

	protected override void Activate()
	{
		player = FindObjectOfType<Player>().gameObject;
		cameraMan.FollowObject(player);
		cameraMan.leadingDistance = 8f;
		cameraMan.timeToReachTarget = 1f;
		ActionComplete();
	}
}