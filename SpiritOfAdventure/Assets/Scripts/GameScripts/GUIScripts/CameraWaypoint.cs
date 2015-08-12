using UnityEngine;
using System.Collections;

public class CameraWaypoint : MonoBehaviour
{
    public float zoom = 5f;

    void OnDrawGizmosSelected()
    {
        float aspect = FindObjectOfType<CameraMan>().GetComponent<Camera>().aspect;

        Vector3 lowerLeft = new Vector3(transform.position.x - zoom * aspect,
                                  transform.position.y - zoom);

        Vector3 upperRight = new Vector3(transform.position.x + zoom * aspect,
                                  transform.position.y + zoom);

        Vector3 upperLeft = new Vector3(lowerLeft.x, upperRight.y);
        Vector3 lowerRight = new Vector3(upperRight.x, lowerLeft.y);

        Gizmos.DrawLine(lowerLeft, lowerRight);
        Gizmos.DrawLine(lowerRight, upperRight);
        Gizmos.DrawLine(upperRight, upperLeft);
        Gizmos.DrawLine(upperLeft, lowerLeft);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "CameraWaypoint.png", false);
    }
}
