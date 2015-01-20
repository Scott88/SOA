using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour {

    public GameObject breakableCluster;

    public float spawnXShift;

    public Vector3 spawnPosition;

    private float timer = 2.0f;
    private bool created = false;

    //private Movement[] players;
    //int playerCount = 0;

	// Use this for initialization
	void Start ()
    {     
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            GameObject.Instantiate(breakableCluster, spawnPosition, new Quaternion());
        }
	}

    void Update()
    {
        if (Network.isServer)
        {
            if (timer > 0.0f)
            {
                timer -= Time.deltaTime;
            }
            else if (!created)
            {
                bool serverFirst = Random.value > 0.5f;

                FindObjectOfType<GameManager>().InitGame();

                Spawn(spawnPosition, serverFirst, 0);
                spawnPosition.x += spawnXShift;

                networkView.RPC("Spawn", RPCMode.OthersBuffered, spawnPosition, !serverFirst, 1);
                spawnPosition.x += spawnXShift;          

                created = true;
            }
        }
    }

    [RPC]
    private void Spawn(Vector3 position, bool isFirst, int team)
    {
        GameObject cluster = Network.Instantiate(breakableCluster, position, new Quaternion(), 0) as GameObject;

        Breakable[] breakables = cluster.GetComponentsInChildren<Breakable>();

        GameManager manager = FindObjectOfType<GameManager>();
        manager.myTeam = team;

        for (int j = 0; j < breakables.Length; j++)
        {
            breakables[j].SetTeam(team);
        }

        if (isFirst)
        {        
            manager.MyTurn();
        }
    }

    //public void AddPlayer(Movement player)
    //{
    //    players[playerCount++] = player;

    //    if (playerCount == 2)
    //    {
    //        int goesFirst;

    //        if (Random.value > 0.5f)
    //        {
    //            goesFirst = 1;
    //        }
    //        else
    //        {
    //            goesFirst = 0;
    //        }

    //        players[goesFirst].myTurn = true;
    //    }
    //}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnPosition, new Vector3(0.5f, 0.5f, 0.5f));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(spawnPosition.x + spawnXShift, spawnPosition.y, spawnPosition.z), new Vector3(0.5f, 0.5f, 0.5f));
    }
}
