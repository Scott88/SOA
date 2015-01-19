using UnityEngine;
using System.Collections;

public class TileManager : MonoBehaviour
{
    private ArrayList occupiedTiles;

    // Use this for initialization
    void Start()
    {
        occupiedTiles = new ArrayList();
    }

    public void OccupyTile(Vector3 position)
    {
        Mathf.Floor(position.x);
        Mathf.Floor(position.y);
        position.z = 0;
        networkView.RPC("NetOccupy", RPCMode.All, position);
    }

    [RPC]
    void NetOccupy(Vector3 position)
    {
        occupiedTiles.Add(position);
    }

    public bool IsOccupied(Vector3 position)
    {
        Mathf.Floor(position.x);
        Mathf.Floor(position.y);
        position.z = 0;

        return occupiedTiles.Contains(position);
    }

    public void FreeTile(Vector3 position)
    {
        networkView.RPC("NetFree", RPCMode.All, position);
    }

    [RPC]
    void NetFree(Vector3 position)
    {
        occupiedTiles.Remove(position);
    }


}
