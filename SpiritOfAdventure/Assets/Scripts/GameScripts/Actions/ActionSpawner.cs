using UnityEngine;
using System.Collections;

public class ActionSpawner : Action
{
	public GameObject Spawned;

	public Vector3 spawnOffset;
	public Vector3 rotationOffset;

	public bool useSpiritPosition = true;

	protected override void Activate()
	{
		ActionComplete();
		Instantiate(Spawned, transform.position + spawnOffset, Quaternion.Euler (rotationOffset));
	}

	protected override void Activate(Vector3 position)
	{
		if (useSpiritPosition)
		{
			ActionComplete();
			Instantiate(Spawned, position + spawnOffset, Quaternion.Euler (rotationOffset));
		}
		else
		{
			Activate();
		}
	}
}