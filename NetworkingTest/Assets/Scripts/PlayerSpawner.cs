using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour {

    public GameObject block;
    public GameObject winBlock;

    private TileManager tileManager;

    private Vector3 spawnPosition;

    private float timer = 2.0f;
    private bool created = false;

	// Use this for initialization
	void Start ()
    {
        spawnPosition = new Vector3(-3.5f, 0.5f);

        tileManager = FindObjectOfType<TileManager>();
        
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            GameObject.Instantiate(block, spawnPosition, new Quaternion());
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
                spawnPosition.x += 5f;

                networkView.RPC("Spawn", RPCMode.OthersBuffered, spawnPosition, !serverFirst, 1);      

                created = true;
            }
        }
    }

    [RPC]
    private void Spawn(Vector3 position, bool isFirst, int team)
    {
        Vector3 blockPosition = position;
        GameObject b = Network.Instantiate(block, blockPosition + Vector3.up, new Quaternion(), 0) as GameObject;
        tileManager.OccupyTile(blockPosition + Vector3.up);
        b.GetComponent<Breakable>().SetTeam(team);

        blockPosition = position;
        b = Network.Instantiate(block, blockPosition + Vector3.right, new Quaternion(), 0) as GameObject;
        tileManager.OccupyTile(blockPosition + Vector3.right);
        b.GetComponent<Breakable>().SetTeam(team);

        blockPosition = position;
        b = Network.Instantiate(block, blockPosition + Vector3.left, new Quaternion(), 0) as GameObject;
        tileManager.OccupyTile(blockPosition + Vector3.left);
        b.GetComponent<Breakable>().SetTeam(team);

        blockPosition = position;
        b = Network.Instantiate(block, blockPosition + Vector3.down, new Quaternion(), 0) as GameObject;
        tileManager.OccupyTile(blockPosition + Vector3.down);
        b.GetComponent<Breakable>().SetTeam(team);

        blockPosition = position;
        b = Network.Instantiate(block, blockPosition + Vector3.up + Vector3.left, new Quaternion(), 0) as GameObject;
        tileManager.OccupyTile(blockPosition + Vector3.up + Vector3.left);
        b.GetComponent<Breakable>().SetTeam(team);

        blockPosition = position;
        b = Network.Instantiate(block, blockPosition + Vector3.up + Vector3.right, new Quaternion(), 0) as GameObject;
        tileManager.OccupyTile(blockPosition + Vector3.up + Vector3.right);
        b.GetComponent<Breakable>().SetTeam(team);

        blockPosition = position;
        b = Network.Instantiate(block, blockPosition + Vector3.down + Vector3.left, new Quaternion(), 0) as GameObject;
        tileManager.OccupyTile(blockPosition + Vector3.down + Vector3.left);
        b.GetComponent<Breakable>().SetTeam(team);

        blockPosition = position;
        b = Network.Instantiate(block, blockPosition + Vector3.down + Vector3.right, new Quaternion(), 0) as GameObject;
        tileManager.OccupyTile(blockPosition + Vector3.down + Vector3.right);
        b.GetComponent<Breakable>().SetTeam(team);

        blockPosition = position;
        b = Network.Instantiate(winBlock, blockPosition, new Quaternion(), 0) as GameObject;
        tileManager.OccupyTile(blockPosition);
        b.GetComponent<WinButton>().SetTeam(team);

        GameManager manager = FindObjectOfType<GameManager>();
        manager.myTeam = team;

        if (isFirst)
        {        
            manager.MyTurn();
        }
    }
}
