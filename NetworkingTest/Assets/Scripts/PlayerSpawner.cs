using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour {

    public GameObject playerPrefab;

    public float spawnXShift;

    public Vector3 spawnPosition;

    private bool hasSpawned = false;

	// Use this for initialization
	void Start ()
    {
       
	}

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            Vector3 position = spawnPosition;
            stream.Serialize(ref position);
        }
        else
        {
            Vector3 position = new Vector3();
            stream.Serialize(ref position);
            spawnPosition = position;
        }
    }

    void OnGUI()
    {
        if (!hasSpawned)
        {
            if (GUI.Button(new Rect(Screen.width * (0.375f), Screen.height * (0.4f), Screen.width * (1f / 4f), Screen.height * (1f / 6f)),
                              "Spawn"))
            {
                Spawn();
            }
        }
    }

    private void Spawn()
    {
        Network.Instantiate(playerPrefab, spawnPosition, new Quaternion(), 0);
        spawnPosition.x += 10;
        hasSpawned = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnPosition, new Vector3(0.5f, 0.5f, 0.5f));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(spawnPosition.x + spawnXShift, spawnPosition.y, spawnPosition.z), new Vector3(0.5f, 0.5f, 0.5f));
    }
}
