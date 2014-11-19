//~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*
//ActionRandomSpawner.cs
//
//By: Nicholas MacDonald
//Spawns a randomly selected object
//~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*

using UnityEngine;
using System.Collections;

public class ActionRandomSpawner : Action
{
	public GameObject[] Spawned;

	public Vector3 spawnOffset;

	//When activated, we spawn a randomly selected object at the center of the game object
	protected override void Activate()
	{
		ActionComplete();
		Instantiate(Spawned[Random.Range(0, Spawned.Length)], transform.position + spawnOffset, transform.rotation);
	}

	//When activated with a contextual position (currently only received from the spirits triggering position), we
	// spawn a randomly selected object at the given position.
	protected override void Activate(Vector3 position)
	{
		ActionComplete();
		Instantiate(Spawned[Random.Range(0, Spawned.Length)], position + spawnOffset, transform.rotation);
	}
}