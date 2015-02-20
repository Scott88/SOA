using UnityEngine;
using System.Collections;

public enum BlockType
{
    BT_NULL,
    BT_WOOD,
    BT_STONE,
    BT_METAL,
    BT_SUPPORT
}

[RequireComponent(typeof(NetworkView))]
public class Breakable : MonoBehaviour
{
    public int health = 1;
    public float minimumSpeed = 3f;

    public float speedDamper = 0.8f;

    public BlockType type;

    public AudioSource collisionSound, brokenSound, debuffSound, healSound;

    public Animator blockAnimator, statusAnimator, putOutAnimator;

    public Renderer statusRenderer;

    public bool fadeOutStatus;
    public float fadeOutDuration;

    private float debuffStartVolume;

    public SpiritType containedSpirit { get; set; }

    private Tile parent;

    private SpiritType statusEffect = SpiritType.ST_NULL;
    private SpiritType pendingEffect = SpiritType.ST_NULL;

    private CashBasherManager manager;

    void Start()
    {
        manager = FindObjectOfType<CashBasherManager>();

        if(debuffSound)
        {
        debuffStartVolume = debuffSound.volume;
            }
    }

    public void SetTile(Tile t)
    {
        parent = t;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.tag == "CannonBall")
        {
            Vector2 normal = coll.contacts[0].normal;

            if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y) && Mathf.Abs(coll.relativeVelocity.x) > minimumSpeed ||
                Mathf.Abs(normal.y) > Mathf.Abs(normal.x) && Mathf.Abs(coll.relativeVelocity.y) > minimumSpeed)
            {
                if (collisionSound)
                {
                    collisionSound.Play();
                }

                NetworkedCannonBall ball = coll.gameObject.GetComponent<NetworkedCannonBall>() as NetworkedCannonBall;

                if (!ball.networkView.isMine || networkView.isMine)
                {
                    return;
                }

                if (IsDebuffedBy(ball.GetEnchantment()))
                {
                    SetStatusEffect(ball.GetEnchantment(), true);
                    ball.Damage();
                }
                else
                {
                    if (Damage())
                    {
                        ball.DamageAndSlow(-coll.relativeVelocity, coll.contacts[0].normal, speedDamper);
                    }
                    else
                    {
                        ball.Damage();
                    }
                }
            }
        }
    }

    [RPC]
    void SetSpirit(int spiritType)
    {
        containedSpirit = (SpiritType)spiritType;

        manager.CreateBlockSpirit(this);
    }

    public bool Damage()
    {
        health--;
        blockAnimator.SetInteger("Health", health);

        if (statusAnimator)
        {
            statusAnimator.SetInteger("Health", health);
        }

        if (health == 0)
        {
            manager.TransferSpirit(containedSpirit, transform.position, networkView.isMine);

            if (networkView.isMine)
            {
                SaveFile.Instance().ModifyBlockInventory(type, -1);
            }

            if (fadeOutStatus)
            {
                StartCoroutine(FadeOutStatus());
            }

            if (brokenSound)
            {
                brokenSound.Play();
            }

            collider2D.enabled = false;

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
        blockAnimator.SetInteger("Health", health);

        if (statusAnimator)
        {
            statusAnimator.SetInteger("Health", health);
        }

        if (health == 0)
        {
            manager.TransferSpirit(containedSpirit, transform.position, networkView.isMine);

            if (networkView.isMine)
            {
                SaveFile.Instance().ModifyBlockInventory(type, -1);
            }

            if (fadeOutStatus)
            {
                StartCoroutine(FadeOutStatus());
            }

            if (brokenSound)
            {
                brokenSound.Play();
            }

            collider2D.enabled = false;
        }
    }

    IEnumerator FadeOutStatus()
    {
        Color color = statusRenderer.material.color;

        while (color.a > 0)
        {
            color.a -= Time.deltaTime / fadeOutDuration;
            statusRenderer.material.color = color;

            if (debuffSound)
            {
                debuffSound.volume = Mathf.Lerp(0, debuffStartVolume, color.a);
            }

            yield return 0;
        }
    }

    public bool IsDying()
    {
        return health == 0;
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

    public bool IsHealedBy(SpiritType spiritType)
    {
        return (statusEffect == SpiritType.ST_GREEN && spiritType == SpiritType.ST_RED ||
               statusEffect == SpiritType.ST_BLUE && spiritType == SpiritType.ST_GREEN ||
               statusEffect == SpiritType.ST_RED && spiritType == SpiritType.ST_BLUE);
    }

    public void SetStatusEffect(SpiritType status, bool immediate)
    {
        if (immediate)
        {
            statusEffect = status;
        }

        pendingEffect = status;

        //AdjustColorsFor(pendingEffect);

        if (status != SpiritType.ST_NULL)
        {
            statusAnimator.SetBool("StatusEffect", true);

            if (debuffSound)
            {
                debuffSound.Play();
            }
        }
        else
        {
            putOutAnimator.SetTrigger("PutOut");

            if (debuffSound)
            {
                debuffSound.Stop();
            }

            if (healSound)
            {
                healSound.Play();
            }
        }

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
        //AdjustColorsFor(pendingEffect);

        if ((SpiritType)spiritType != SpiritType.ST_NULL)
        {
            statusAnimator.SetBool("StatusEffect", true);

            if (debuffSound)
            {
                debuffSound.Play();
            }
        }
        else
        {
            putOutAnimator.SetTrigger("PutOut");

            if (debuffSound)
            {
                debuffSound.Stop();
            }

            if (healSound)
            {
                healSound.Play();
            }
        }
    }

    public void Tick()
    {
        if (statusEffect != SpiritType.ST_NULL)
        {
            Damage();

            if (debuffSound)
            {
                debuffSound.Play();
            }
        }
    }

    public void Apply()
    {
        statusEffect = pendingEffect;
    }

    public void PutOut()
    {
        statusAnimator.SetBool("StatusEffect", false);
    }

    public void FallTo(Vector3 position, float acceleration, bool takeDamage)
    {
        StartCoroutine(FallToAnim(position, acceleration, takeDamage));

        networkView.RPC("NetFallTo", RPCMode.Others, position, acceleration);
    }

    [RPC]
    void NetFallTo(Vector3 position, float acceleration)
    {
        StartCoroutine(FallToAnim(position, acceleration, false));
    }

    IEnumerator FallToAnim(Vector3 position, float acceleration, bool takeDamage)
    {
        float speed = 0;

        while (transform.position.y > position.y)
        {
            speed += Time.deltaTime * acceleration;
            transform.Translate(Vector3.down * speed * Time.deltaTime);

            if (transform.position.y < position.y)
            {
                transform.position = position;

                if (takeDamage)
                {
                    Damage();
                }
            }

            yield return 0;
        }
    }
}
