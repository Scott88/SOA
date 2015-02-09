using UnityEngine;
using System.Collections;

public class NetworkedCannonBall : MonoBehaviour
{
    public int health;

    [RPC]
    public void SetVelocity(Vector3 velocity)
    {
        rigidbody2D.velocity = velocity;
    }

    public void Damage()
    {
        health--;

        if (health == 0)
        {
            Destroy(gameObject);
        }
    }
}
