using UnityEngine;
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

    private int startingHealth;

    private Tile parent;

    private SpiritType statusEffect = SpiritType.ST_NULL;
    private SpiritType pendingEffect = SpiritType.ST_NULL;

    private Color startColor;

    void Start()
    {
        startingHealth = health;
        startColor = renderer.material.color;
    }

    public void SetTile(Tile t)
    {
        parent = t;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.tag == "CannonBall")
        {
            NetworkedCannonBall ball = coll.gameObject.GetComponent<NetworkedCannonBall>() as NetworkedCannonBall;

            if (!ball.networkView.isMine || networkView.isMine)
            {
                return;
            }

            if (coll.relativeVelocity.magnitude > minimumSpeed)
            {
                if (IsDebuffedBy(ball.GetEnchantment()))
                {
                    SetStatusEffect(ball.GetEnchantment(), true);
                    ball.Damage();
                }
                else
                {
                    if (Damage())
                    {
                        ball.DamageAndSlow(-coll.relativeVelocity, transform.position, speedDamper);
                    }
                    else
                    {
                        ball.Damage();
                    }
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

        Color color = renderer.material.color;
        color.a = (float)health / (float)startingHealth;
        renderer.material.color = color;

        networkView.RPC("NetDamage", RPCMode.Others);

        return false;
    }

    [RPC]
    public void NetDamage()
    {
        health--;

        if (health == 0)
        {
            SaveFile.Instance().ModifyBlockInventory(type, -1);
            Destroy(gameObject);
        }

        Color color = renderer.material.color;
        color.a = (float)health / (float)startingHealth;
        renderer.material.color = color;
    }

    public SpiritType GetStatusEffect()
    {
        return statusEffect;
    }

    public bool IsDebuffedBy(SpiritType spiritType)
    {
        return type == BlockType.BT_WOOD && spiritType == SpiritType.ST_RED ||
               type == BlockType.BT_STONE && spiritType == SpiritType.ST_GREEN ||
               type == BlockType.BT_METAL && spiritType == SpiritType.ST_BLUE;
    }

    public void SetStatusEffect(SpiritType status, bool immediate)
    {
        if (immediate)
        {
            statusEffect = status;
        }

        pendingEffect = status;
        AdjustColorsFor(pendingEffect);
        networkView.RPC("NetSetStatusEffect", RPCMode.Others, (int)status, immediate);
    }

    [RPC]
    void NetSetStatusEffect(int spiritType, bool immediate)
    {
        if (immediate)
        {
            statusEffect = (SpiritType)spiritType;
        }

        pendingEffect = (SpiritType)spiritType;
        AdjustColorsFor(pendingEffect);
    }

    void AdjustColorsFor(SpiritType type)
    {
        Color color = renderer.material.color;

        if (type == SpiritType.ST_NULL)
        {
            color = startColor;
        }
        else
        {
            if (type != SpiritType.ST_GREEN)
            {
                color.g *= 0.5f;
            }

            if (type != SpiritType.ST_BLUE)
            {
                color.b *= 0.5f;
            }

            if (type != SpiritType.ST_RED)
            {
                color.r *= 0.5f;
            }
        }

        renderer.material.color = color;
    }

    public void Tick()
    {
        if (statusEffect != SpiritType.ST_NULL)
        {
            Damage();
        }
    }

    public void Apply()
    {
        statusEffect = pendingEffect;
    }
}
