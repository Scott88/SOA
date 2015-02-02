using UnityEngine;
using System.Collections;

public class NetworkedCannonBall : MonoBehaviour
{
    [RPC]
    public void SetVelocity(Vector3 velocity)
    {
        rigidbody2D.velocity = velocity;
    }
}
