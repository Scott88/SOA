using UnityEngine;
using System.Collections;

public class SmoothDamper : MonoBehaviour
{
    public Vector3 target { get; set; }

    private Vector3 velocity;

    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, 0.1f);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            Destroy(gameObject);
        }
    }

}
