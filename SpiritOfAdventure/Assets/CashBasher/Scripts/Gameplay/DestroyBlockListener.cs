using UnityEngine;
using System.Collections;

public class DestroyBlockListener : MonoBehaviour {

    void DestroyBlock()
    {
        Destroy(transform.parent.gameObject);
    }
}
