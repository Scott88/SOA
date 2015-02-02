using UnityEngine;
using System.Collections;

public class BreakableTrigger : Triggerable
{
    public float minimumSpeed = 0f;
    public int health = 1;

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.tag == "CannonBall")
        {
            if (coll.relativeVelocity.magnitude > minimumSpeed)
            {
                health--;
            }

            if (health == 0)
            {
                Trigger();
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "CannonBall")
        {
            if (coll.rigidbody2D.velocity.magnitude > minimumSpeed)
            {
                health--;
            }

            if (health == 0)
            {
                Trigger();
                Destroy(gameObject);
            }
        }
    }
}
