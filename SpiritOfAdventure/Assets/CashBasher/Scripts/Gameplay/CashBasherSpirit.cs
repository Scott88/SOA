using UnityEngine;
using System.Collections;

public class CashBasherSpirit : MonoBehaviour
{
    public SpiritType type;

    public GameObject fizzleEffect;

    public CashBasherSpiritGUI gui;

    public GameObject healAOE;

    private bool active = false;

    private bool retreating = false;

    private CashBasherManager manager;

    private Vector3 targetPosition;
    private Vector3 targetScale;
    private float targetVolume;

    private NetworkedCannon targetCannon;

    private NetworkedTileSet targetSet;

    private Vector3 velocityRef;
    private Vector3 scaleRef;
    private float volumeRef;

    private bool poofOnArrival;

    private bool consume;

    void Start()
    {
        manager = FindObjectOfType<CashBasherManager>();
    }

    public void Activate(Vector3 spawnPoint)
    {
        active = true;
        retreating = false;

        GetComponent<AudioSource>().Play();

        transform.position = spawnPoint;

        targetPosition = transform.position + Vector3.up * 2f;
        targetScale = Vector3.one;

        GetComponent<Collider2D>().enabled = true;

        GetComponent<NetworkView>().RPC("NetActivate", RPCMode.Others, spawnPoint);
    }

    [RPC]
    void NetActivate(Vector3 spawnPoint)
    {
        active = true;
        retreating = false;

        GetComponent<AudioSource>().Play();

        transform.position = spawnPoint;

        targetPosition = transform.position + Vector3.up * 2f;
        targetScale = Vector3.one;

        GetComponent<Collider2D>().enabled = true;
    }

    public void Deactivate()
    {
        active = false;
        retreating = false;
        GetComponent<AudioSource>().Stop();

        GetComponent<Collider2D>().enabled = false;

        poofOnArrival = false;

        transform.localScale = Vector3.zero;
    }

    public void Retreat(Vector3 spawnPoint)
    {
        retreating = true;
        targetPosition = spawnPoint;
        targetScale = Vector3.zero;

        GetComponent<Collider2D>().enabled = false;

        poofOnArrival = false;

        GetComponent<NetworkView>().RPC("NetRetreat", RPCMode.Others, spawnPoint);
    }

    [RPC]
    void NetRetreat(Vector3 spawnPoint)
    {
        retreating = true;
        targetPosition = spawnPoint;
        targetScale = Vector3.zero;

        GetComponent<Collider2D>().enabled = false;

        poofOnArrival = false;
    }

    public void MoveHereAndPoof(Vector3 position)
    {
        poofOnArrival = true;

        targetPosition = position;
        targetPosition.z = -30f;

        GetComponent<Collider2D>().enabled = false;

        GetComponent<NetworkView>().RPC("NetMoveHereAndPoof", RPCMode.Others, position);
    }

    [RPC]
    void NetMoveHereAndPoof(Vector3 position)
    {
        poofOnArrival = true;

        targetPosition = position;
        targetPosition.z = -30f;

        GetComponent<Collider2D>().enabled = false;      
    }

    public void MoveHereAndTrigger(bool serverCannon)
    {
        poofOnArrival = true;
        consume = serverCannon == Network.isServer;

        targetCannon = manager.GetCannon(serverCannon);

        targetPosition = targetCannon.transform.position;
        targetPosition.z = -30f;

        GetComponent<Collider2D>().enabled = false;

        GetComponent<NetworkView>().RPC("NetMoveHereAndTrigger", RPCMode.Others, serverCannon);
    }

    [RPC]
    void NetMoveHereAndTrigger(bool serverCannon)
    {
        poofOnArrival = true;
        consume = serverCannon == Network.isServer;

        targetCannon = manager.GetCannon(serverCannon);

        targetPosition = targetCannon.transform.position;
        targetPosition.z = -30f;

        GetComponent<Collider2D>().enabled = false;
    }

    public void MoveHereAndHeal(Vector3 position, bool serverSet)
    {
        poofOnArrival = true;
        consume = serverSet == Network.isServer;

        targetSet = manager.GetTileSet(serverSet);

        targetPosition = position;
        targetPosition.z = -30f;

        GetComponent<Collider2D>().enabled = false;

        GetComponent<NetworkView>().RPC("NetMoveHereAndHeal", RPCMode.Others, position, serverSet);
    }

    [RPC]
    void NetMoveHereAndHeal(Vector3 position, bool serverSet)
    {
        poofOnArrival = true;
        consume = serverSet == Network.isServer;

        targetSet = manager.GetTileSet(serverSet);

        targetPosition = position;
        targetPosition.z = -30f;

        GetComponent<Collider2D>().enabled = false;
    }

    public void MoveHere(Vector3 target)
    {
        targetPosition = target; 
        targetPosition.z = -30f;
    }

    void Poof()
    {
        active = false;

        if (targetCannon)
        {
            if (consume)
            {
                gui.Remove();
            }

            targetCannon.ApplyBuff(type);
        }
        else if (targetSet)
        {
            if (consume)
            {
                gui.Remove();
            }

            targetSet.HealFrom(targetPosition, type);
        }
        else
        {
            GameObject tempParticle = (GameObject)Instantiate(fizzleEffect, transform.position + Vector3.back * 3f, Quaternion.identity);
            tempParticle.GetComponent<ParticleSystem>().Play();
        }

        transform.localScale = Vector3.zero;

        GetComponent<ParticleSystem>().Stop();
        GetComponent<AudioSource>().Stop();

        poofOnArrival = false;
    }

    void FixedUpdate()
    {
        if (active)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocityRef, 0.4f, Mathf.Infinity, Time.fixedDeltaTime);

            if (velocityRef.x < 0 && targetScale.x > 0 ||
                velocityRef.x > 0 && targetScale.x < 0)
            {
                targetScale.x = -targetScale.x;
            }

            transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref scaleRef, 0.2f, Mathf.Infinity, Time.fixedDeltaTime);
            GetComponent<AudioSource>().volume = Mathf.SmoothDamp(GetComponent<AudioSource>().volume, targetVolume, ref volumeRef, 0.2f, Mathf.Infinity, Time.fixedDeltaTime);

            if (retreating && transform.localScale.x < 0.01f)
            {
                Deactivate();
            }

            if (Vector3.Distance(transform.position, targetPosition) < 1f && poofOnArrival)
            {
                Poof();
                targetCannon = null;
            }
        }
    }
}
