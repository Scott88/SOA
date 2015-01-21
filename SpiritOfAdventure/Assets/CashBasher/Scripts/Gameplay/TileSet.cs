using UnityEngine;
using System.Collections;

public class TileSet : MonoBehaviour {

    public Vector3 minCoord, maxCoord;

    public int team;

    public bool CanPlace(Vector3 position, int t)
    {
        if (t != team)
        { 
            return false;
        }
        else
        {
            return minCoord.x < position.x && minCoord.y < position.y &&
                   maxCoord.x > position.x && maxCoord.y > position.y;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(minCoord, new Vector3(minCoord.x, maxCoord.y));
        Gizmos.DrawLine(maxCoord, new Vector3(minCoord.x, maxCoord.y));
        Gizmos.DrawLine(minCoord, new Vector3(maxCoord.x, minCoord.y));
        Gizmos.DrawLine(maxCoord, new Vector3(maxCoord.x, minCoord.y));
    }
}
