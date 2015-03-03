using UnityEngine;
using System.Collections;

public class NetworkedCannonBall : MonoBehaviour
{
    public int health;

    private SpiritType enchantment = SpiritType.ST_NULL;

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

    [RPC]
    public void NetDamage()
    {
        health--;

        if (health == 0)
        {
            Destroy(gameObject);
        }
    }

    [RPC]
    public void Enchant(int spiritType)
    {
        enchantment = (SpiritType)spiritType;

        Color color = renderer.material.color;

        if (enchantment != SpiritType.ST_GREEN)
        {
            color.g = 0.5f;
        }

        if (enchantment != SpiritType.ST_BLUE)
        {
            color.b = 0.5f;
        }

        if (enchantment != SpiritType.ST_RED)
        {
            color.r = 0.5f;
        }

        renderer.material.color = color;
    }

    public SpiritType GetEnchantment()
    {
        return enchantment;
    }

    public void DamageAndSlow(Vector3 startSpeed, Vector3 normal, float speedDamper)
    {
        health--;

        if (health == 0)
        {
            networkView.RPC("NetDamageAndSlow", RPCMode.Others, startSpeed, normal, speedDamper);
            Destroy(gameObject);

            return;
        }

        Vector2 velocity = startSpeed;

        if (Mathf.Abs(normal.x) >= Mathf.Abs(normal.y))
        {
            velocity.x *= speedDamper;
        }
        else
        {
            velocity.y *= speedDamper;
        }

        rigidbody2D.velocity = velocity;

        networkView.RPC("NetDamageAndSlow", RPCMode.Others, (Vector3)velocity);
    }

    [RPC]
    public void NetDamageAndSlow(Vector3 velocity)
    {
        health--;

        if (health == 0)
        {
            Destroy(gameObject);
            return;
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
