using UnityEngine;
using System.Collections;

public class ActionCameraShake : Action
{
	public float intensity = 2f;
	public float time = 1f;

	private CameraMan cameraMan;

	void Start()
	{
		cameraMan = FindObjectOfType<CameraMan>() as CameraMan;
	}

	protected override void Activate()
	{
		cameraMan.ShakeCamera(intensity, intensity / time);
		ActionComplete();
	}
}
