using UnityEngine;
using System.Collections;

public class CashBasherSpirit : MonoBehaviour
{
    public SpiritType type;

    public GameObject fizzleEffect;

    private bool active = false;

    private bool retreating = false;

    private CashBasherManager manager;

    private Vector3 targetPosition;
    private Vector3 targetScale;
    private float targetVolume;

    private NetworkedCannon targetCannon;

    private Vector3 velocityRef;
    private Vector3 scaleRef;
    private float volumeRef;

    private bool poofOnArrival;

    void Start()
    {
        manager = FindObjectOfType<CashBasherManager>();
    }

    public void Activate(Vector3 spawnPoint)
    {
        active = true;
        retreating = false;

        audio.Play();

        transform.position = spawnPoint;

        targetPosition = transform.position + Vector3.up * 2f;
        targetScale = Vector3.one;

        collider2D.enabled = true;

        networkView.RPC("NetActivate", RPCMode.Others, spawnPoint);
    }

    [RPC]
    void NetActivate(Vector3 spawnPoint)
    {
        active = true;
        retreating = false;

        audio.Play();

        transform.position = spawnPoint;

        targetPosition = transform.position + Vector3.up * 2f;
        targetScale = Vector3.one;

        collider2D.enabled = true;
    }

    public void Deactivate()
    {
        active = false;
        retreating = false;
        audio.Stop();

        networkView.RPC("NetDeactivate", RPCMode.Others);
    }

    void NetDeactivate()
    {
        active = false;
        retreating = false;
        audio.Stop();
    }

    public void Retreat(Vector3 spawnPoint)
    {
        retreating = true;
        targetPosition = spawnPoint;
        targetScale = Vector3.zero;
    }

    public void MoveHereAndPoof(Vector3 position)
    {
        poofOnArrival = true;

        targetPosition = position;
        targetPosition.z = -30f;

        collider2D.enabled = false;

        networkView.RPC("NetMoveHereAndPoof", RPCMode.Others, position);
    }

    [RPC]
    void NetMoveHereAndPoof(Vector3 position)
    {
        poofOnArrival = true;

        targetPosition = position;
        targetPosition.z = -30f;

        collider2D.enabled = false;      
    }

    public void MoveHereAndTrigger(bool serverCannon)
    {
        poofOnArrival = true;

        targetCannon = manager.GetCannon(serverCannon);

        targetPosition = targetCannon.transform.position;
        targetPosition.z = -30f;

        collider2D.enabled = false;

        networkView.RPC("NetMoveHereAndTrigger", RPCMode.Others, serverCannon);
    }

    [RPC]
    void NetMoveHereAndTrigger(bool serverCannon)
    {
        poofOnArrival = true;

        targetCannon = manager.GetCannon(serverCannon);

        targetPosition = targetCannon.transform.position;
        targetPosition.z = -30f;

        collider2D.enabled = false;
    }

    void Poof()
    {
        active = false;

        if (targetCannon)
        {
            targetCannon.ApplyEffect(type);
        }
        else
        {
            GameObject tempParticle = (GameObject)Instantiate(fizzleEffect, transform.position + Vector3.back * 3f, Quaternion.identity);
            tempParticle.particleSystem.Play();
        }

        transform.localScale = Vector3.zero;

        particleSystem.Stop();
        audio.Stop();
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
            audio.volume = Mathf.SmoothDamp(audio.volume, targetVolume, ref volumeRef, 0.2f, Mathf.Infinity, Time.fixedDeltaTime);

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
