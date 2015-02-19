using UnityEngine;
using System.Collections;

public class PutOutListener : MonoBehaviour {

    public Breakable parent;

    void PutOut()
    {
        parent.PutOut();
    }
}
