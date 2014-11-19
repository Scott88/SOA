using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour 
{
	public int counter;

	private GameObject player;
	private GameObject pickup;

	void Start () 
	{
		player = FindObjectOfType<Player>().gameObject;
		pickup = FindObjectOfType<PickUp>().gameObject;
		counter = 0;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other == player.collider2D)
		{

			counter++;
			//display counter at end of game
		}
	}
	

	void Update () 
	{

	}
}
