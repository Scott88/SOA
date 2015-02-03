using UnityEngine;
using System.Collections;

public class NetworkedCannonBall : MonoBehaviour
{
    public int health;

    private CashBasherManager manager;

    void Start()
    {
        manager = FindObjectOfType<CashBasherManager>() as CashBasherManager;
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
            if (networkView.isMine)
            {        
                Network.Destroy(gameObject);
            }
        }
    }

    void OnDestroy()
    {
        if (networkView.isMine)
        {
            manager.ReadyNextTurn();
        }
    }
}
