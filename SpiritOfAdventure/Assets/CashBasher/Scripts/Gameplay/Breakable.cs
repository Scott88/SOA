﻿using UnityEngine;
using System.Collections;

public enum BlockType
{
    BT_NULL,
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
                if (Damage())
                {
                    ball.DamageAndSlow(coll.relativeVelocity, transform.position, speedDamper);
                }
                else
                {
                    ball.Damage();
                }     
            }
        }
    }

    public bool Damage()
    {
        health--;

        if (health == 0)
        {
            Destroy(gameObject);
            networkView.RPC("NetDamage", RPCMode.Others);

            return true;
        }

        networkView.RPC("NetDamage", RPCMode.Others);

        return false;
    }

    [RPC]
    public void NetDamage()
    {
        health--;

        if (health == 0)
        {
            Destroy(gameObject);
        }
    }
}
