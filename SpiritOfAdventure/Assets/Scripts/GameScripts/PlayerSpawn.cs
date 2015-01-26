using UnityEngine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{
    public int pathNumber;

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "LilySpawn.png", false);
    }

}
