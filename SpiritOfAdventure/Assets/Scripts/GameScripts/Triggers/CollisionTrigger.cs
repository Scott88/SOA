using UnityEngine;

public class CollisionTrigger : Triggerable
{
	public GameObject target;

	void OnCollisionEnter2D(Collision2D coll)
	{
		if(target)
		{
			if (coll.collider == target.GetComponent<Collider2D>())
			{
				Trigger();
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(target)
		{
			if (other == target.GetComponent<Collider2D>())
			{
				Trigger();
			}
		}
	}

	protected override void OnDrawGizmosSelected()
	{
		if(target)
		{
			Gizmos.DrawIcon(target.transform.position, "CollisionTrigger.png", false);
		}

		base.OnDrawGizmosSelected();
	}
}