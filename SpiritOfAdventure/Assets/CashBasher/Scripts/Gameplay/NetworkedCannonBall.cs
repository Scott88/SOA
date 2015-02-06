using UnityEngine;
using System.Collections;

public class NetworkedCannonBall : MonoBehaviour
{
    public int health;

    private CashBasherManager manager;

    void Start()
    {
        manager = FindObjectOfType<CashBasherManager>() as CashBasherManager;

        manager.cameraMan.FollowObject(gameObject);
    }

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

        networkView.RPC("NetDamage", RPCMode.Others);
    }

    public void NetDamage()
    {
        health--;

        if (health == 0)
        {
            Destroy(gameObject);
        }
    }

    public void DamageAndSlow(Vector3 startSpeed, Vector3 blockPos, float speedDamper)
    {
        health--;

        if (health == 0)
        {
            networkView.RPC("NetDamageAndSlow", RPCMode.Others, startSpeed, blockPos, speedDamper);
            Destroy(gameObject);

            return;
        }

        Vector3 direction = blockPos - transform.position;

        Vector2 velocity = startSpeed;

        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            velocity.x *= speedDamper;
        }
        else
        {
            velocity.y *= speedDamper;
        }

        rigidbody2D.velocity = velocity;

        networkView.RPC("NetDamageAndSlow", RPCMode.Others, startSpeed, blockPos, speedDamper);
    }

    [RPC]
    public void NetDamageAndSlow(Vector3 blockPos, float speedDamper)
    {
        health--;

        if (health == 0)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = blockPos - transform.position;

        Vector2 velocity = rigidbody2D.velocity;

        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            velocity.x *= speedDamper;
        }
        else
        {
            velocity.y *= speedDamper;
        }

        rigidbody2D.velocity = velocity;
    }

    void OnDestroy()
    {
        if (networkView.isMine)
        {         
            manager.ReadyNextTurn();
        }
    }
}
