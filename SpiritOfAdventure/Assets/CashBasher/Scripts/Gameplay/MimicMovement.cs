using UnityEngine;
using System.Collections;

public class MimicMovement : MonoBehaviour
{
    public GameObject target;

    public float movementScale = 1f;

    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = target.transform.position;
    }

    void Update()
    {
        transform.Translate((target.transform.position - lastPosition) * movementScale);

        lastPosition = target.transform.position;
    }
}
