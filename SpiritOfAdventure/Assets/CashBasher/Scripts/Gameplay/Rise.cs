using UnityEngine;
using System.Collections;

public class Rise : MonoBehaviour
{
    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0f, speed * Time.deltaTime, 0f);
    }
}
