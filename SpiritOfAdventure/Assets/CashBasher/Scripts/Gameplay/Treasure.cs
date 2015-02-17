using UnityEngine;
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

            Vector2 normal = coll.contacts[0].normal;

            if (Mathf.Abs(normal.x) > 0 && Mathf.Abs(coll.relativeVelocity.x) > minimumSpeed ||
                Mathf.Abs(normal.y) > 0 && Mathf.Abs(coll.relativeVelocity.y) > minimumSpeed)
            {
                Damage();
                ball.DamageAndSlow(-coll.relativeVelocity, coll.contacts[0].normal, speedDamper);             
            }
        }
    }

    public void Damage()
    {
        FindObjectOfType<CashBasherManager>().SwitchToState((int)GamePhase.GP_WIN);
        Instantiate(coinExplosion, transform.position + Vector3.back * 5, Quaternion.Euler(270f, 0f, 0f));
        Destroy(gameObject);
        networkView.RPC("NetDamage", RPCMode.Others);
    }

    [RPC]
    public void NetDamage()
    {
        FindObjectOfType<CashBasherManager>().SwitchToState((int)GamePhase.GP_LOSE);
        Instantiate(coinExplosion, transform.position + Vector3.back * 5, Quaternion.Euler(270f, 0f, 0f));
        Destroy(gameObject);
    }
}
