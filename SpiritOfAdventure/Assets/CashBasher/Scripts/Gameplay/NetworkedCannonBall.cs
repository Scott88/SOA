using UnityEngine;
using System.Collections;

public class NetworkedCannonBall : MonoBehaviour
{
    public int health;

    public int splitCount;

    public float speedVariance;

    public GameObject splitBall;

    public GameObject splitPoof;

    private SpiritType enchantment = SpiritType.ST_NULL;

    private CashBasherManager manager;

    private Vector3 previousVel;

    void Start()
    {
        manager = FindObjectOfType<CashBasherManager>() as CashBasherManager;

        manager.cameraMan.FollowObject(gameObject);
    }

    void Update()
    {
        if (networkView.isMine)
        {
            previousVel = rigidbody2D.velocity;
        }
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
            networkView.RPC("NetDamageAndSlow", RPCMode.Others, Vector3.zero);
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
    void NetDamageAndSlow(Vector3 velocity)
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
        Instantiate(splitPoof, transform.position, Quaternion.identity);

        if (networkView.isMine)
        {
            if (splitCount > 0)
            {
                for (int j = 0; j < splitCount; j++)
                {
                    Vector3 randomPosition = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
                    GameObject ball = Network.Instantiate(splitBall, transform.position + randomPosition, Quaternion.identity, 0) as GameObject;

                    Vector3 velocity = new Vector3(Random.Range(-speedVariance, speedVariance), Random.Range(-speedVariance, speedVariance));

                    ball.networkView.RPC("SetVelocity", RPCMode.All, velocity + previousVel);

                    if (enchantment != SpiritType.ST_NULL)
                    {
                        ball.networkView.RPC("Enchant", RPCMode.All, (int)enchantment);
                    }
                } 
            }
            else
            {
                manager.ReadyNextTurn();
            }
        }
    }
}
