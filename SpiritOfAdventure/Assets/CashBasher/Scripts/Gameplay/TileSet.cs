using UnityEngine;
using System.Collections;

public class TileSet : MonoBehaviour {

    public Vector3 minCoord, maxCoord;

    public Vector3 minTreasure, maxTreasure;

    private ArrayList blocks;

    // Use this for initialization
    void Start()
    {
        blocks = new ArrayList();
    }

    public bool CanPlace(Vector3 position)
    {
        return minCoord.x < position.x && minCoord.y < position.y &&
               maxCoord.x > position.x && maxCoord.y > position.y &&
               !(minTreasure.x < position.x && minTreasure.y < position.y &&
               maxTreasure.x > position.x && maxTreasure.y > position.y);
    }

    public Vector3 GetPositionFromCoords(int x, int y)
    {
        return new Vector3(minCoord.x + (float)x + 0.5f, minCoord.y + (float)y + 0.5f);
    }

    public BlockType PlaceBlock(Vector3 position, GameObject block)
    {
        position.x = Mathf.Floor(position.x) + 0.5f;
        position.y = Mathf.Floor(position.y) + 0.5f;
        position.z = 0;

        GameObject placed = Instantiate(block, position, new Quaternion()) as GameObject;
        blocks.Add(placed);

        Breakable breakable = placed.GetComponent<Breakable>();

        return breakable.type;
    }

    public void RemoveBlock(GameObject block)
    {
        blocks.Remove(block);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(minCoord, new Vector3(minCoord.x, maxCoord.y));
        Gizmos.DrawLine(maxCoord, new Vector3(minCoord.x, maxCoord.y));
        Gizmos.DrawLine(minCoord, new Vector3(maxCoord.x, minCoord.y));
        Gizmos.DrawLine(maxCoord, new Vector3(maxCoord.x, minCoord.y));

        Gizmos.color = Color.yellow;

        Gizmos.DrawCube((minTreasure + maxTreasure) / 2f, (maxTreasure - minTreasure));
    }
}
