using UnityEngine;

public class TaggedCollisionTrigger : Triggerable
{
    public string target;

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.tag == target)
        {
            Trigger();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == target)
        {
            Trigger();
        }       
    }
}