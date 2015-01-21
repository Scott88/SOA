using UnityEngine;
using System.Collections;

public class NetLevelHandshake : MonoBehaviour {

	// Use this for initialization
    void Awake()
    {
        FindObjectOfType<NetworkedLevelLoader>().Ready();
    }
}
