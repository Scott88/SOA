using UnityEngine;
using System.Collections;

public class SmoothDamper : MonoBehaviour
{
    public float delay { get; set; }

    public Vector3 target { get; set; }

    public float duration { get; set; }

    private Vector3 velocity;

    void Awake()
    {
        delay = 0;
        duration = 0.1f;
    }

    void Update()
    {
        delay -= Time.deltaTime;

        if (delay <= 0)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, duration);

            if (Vector3.Distance(transform.position, target) < 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }

}
