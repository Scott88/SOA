using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour {

    public GameObject playerPrefab;

    public float spawnXShift;

    public Vector3 spawnPosition;

	// Use this for initialization
	void Start ()
    {
        if (Network.peerType == NetworkPeerType.Server)
        {
            Spawn(spawnPosition);
            spawnPosition.x += spawnXShift;

            for (int j = 0; j < Network.connections.Length; j++)
            {
                networkView.RPC("Spawn", Network.connections[j], spawnPosition);
                spawnPosition.x += spawnXShift;
            }
        }
        else if (Network.peerType == NetworkPeerType.Disconnected)
        {
            GameObject.Instantiate(playerPrefab, spawnPosition, new Quaternion());
        }
	}

    [RPC]
    private void Spawn(Vector3 position)
    {
        Network.Instantiate(playerPrefab, position, new Quaternion(), 0);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnPosition, new Vector3(0.5f, 0.5f, 0.5f));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(spawnPosition.x + spawnXShift, spawnPosition.y, spawnPosition.z), new Vector3(0.5f, 0.5f, 0.5f));
    }
}
