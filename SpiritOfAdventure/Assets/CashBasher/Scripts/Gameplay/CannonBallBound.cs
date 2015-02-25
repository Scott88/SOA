using UnityEngine;
using System.Collections;

public class CannonBallBound : MonoBehaviour
{
    CameraMan cameraMan;

    void Start()
    {
        cameraMan = FindObjectOfType<CameraMan>();
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.tag == "CannonBall")
        {
            cameraMan.StopFollowing();

            coll.gameObject.GetComponent<SelfDeleter>().lifeTime = 1f;  
        }
    }

}
