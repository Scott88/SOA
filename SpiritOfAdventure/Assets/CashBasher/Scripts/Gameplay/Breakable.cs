using UnityEngine;
using System.Collections;

public enum BlockType
{
    BT_WOOD,
    BT_STONE,
    BT_METAL
}

[RequireComponent(typeof(NetworkView))]
public class Breakable : MonoBehaviour
{
    public int health = 1;
    public float minimumSpeed = 3f;

    public float speedDamper = 0.8f;

    public BlockType type;

    void Start()
    {
        if (health == 1)
        {
            collider2D.isTrigger = true;
        }
        else
        {
            collider2D.isTrigger = false;
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.tag == "CannonBall")
        {
            NetworkedCannonBall ball = coll.gameObject.GetComponent<NetworkedCannonBall>() as NetworkedCannonBall;

            if (!ball.networkView.isMine)
            {
                return;
            }

            if (coll.relativeVelocity.magnitude > minimumSpeed)
            {
                ball.networkView.RPC("Damage", RPCMode.All, transform.position, speedDamper);
                networkView.RPC("Damage", RPCMode.All);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "CannonBall")
        {
            NetworkedCannonBall ball = coll.GetComponent<NetworkedCannonBall>() as NetworkedCannonBall;

            if (!ball.networkView.isMine)
            {
                return;
            }

            if (coll.rigidbody2D.velocity.magnitude > minimumSpeed)
            {
                ball.networkView.RPC("Damage", RPCMode.All, transform.position, speedDamper);
                networkView.RPC("Damage", RPCMode.All);
            }
        }
    }

    [RPC]
    public void Damage()
    {
        health--;

        if (health == 1)
        {
            collider2D.isTrigger = true;
        }
        else if (health == 0)
        {
            Destroy(gameObject);
        }
    }
}
