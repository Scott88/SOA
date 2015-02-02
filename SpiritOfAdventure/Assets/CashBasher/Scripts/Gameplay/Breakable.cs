using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class Breakable : MonoBehaviour {

    public int health;
    public float minimumSpeed;

    private int team = -1;

    void Start()
    {
        FindObjectOfType<CashBasherManager>().AddBlock(this);
    }

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
                health--;
            }

            if (health == 0)
            {
                Break();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "CannonBall")
        {
            NetworkedCannonBall ball = coll.GetComponent<NetworkedCannonBall>() as NetworkedCannonBall;

            if (!ball.networkView.isMine)
            {
                return;
            }

            if (coll.rigidbody2D.velocity.magnitude > minimumSpeed)
            {
                health--;
            }

            if (health == 0)
            {
                Break();
            }
        }
    }

    [RPC]
    public void Break()
    {
        Destroy(gameObject);          
    }

    public void SetTeam(int t)
    {
        networkView.RPC("NetSetTeam", RPCMode.All, t);
    }

    [RPC]
    void NetSetTeam(int t)
    {
        team = t;
    }

    public int GetTeam()
    {
        return team;
    }
}
