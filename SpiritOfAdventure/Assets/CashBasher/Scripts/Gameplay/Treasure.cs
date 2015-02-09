﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class Treasure : MonoBehaviour
{
    public float minimumSpeed = 3f;

    public float speedDamper = 0.8f;

    public GameObject coinExplosion;

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
                Damage();
                ball.DamageAndSlow(-coll.relativeVelocity, transform.position, speedDamper);             
            }
        }
    }

    public void Damage()
    {
        FindObjectOfType<CashBasherManager>().SwitchToState((int)GamePhase.GP_WIN);
        Instantiate(coinExplosion, transform.position, Quaternion.Euler(270f, 0f, 0f));
        Destroy(gameObject);
        networkView.RPC("NetDamage", RPCMode.Others);
    }

    [RPC]
    public void NetDamage()
    {
        FindObjectOfType<CashBasherManager>().SwitchToState((int)GamePhase.GP_LOSE);
        Instantiate(coinExplosion, transform.position, Quaternion.Euler(270f, 0f, 0f));
        Destroy(gameObject);
    }
}