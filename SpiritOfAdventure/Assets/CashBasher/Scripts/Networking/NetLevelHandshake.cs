using UnityEngine;
using System.Collections;

public class NetLevelHandshake : MonoBehaviour {

    public int myCostume;

	// Use this for initialization
    void Awake()
    {
        FindObjectOfType<NetworkedLevelLoader>().Ready(myCostume);
    }
}
