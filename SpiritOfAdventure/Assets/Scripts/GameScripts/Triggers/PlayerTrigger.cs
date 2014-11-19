using UnityEngine;

public class PlayerTrigger : Triggerable
{
	protected GameManager gameManager;

	void Awake()
	{
		gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			Trigger();
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawIcon(transform.position, "TriggerIcon.png", false);
	}
}