using UnityEngine;
using System.Collections;

public class TileSet : MonoBehaviour
{
    public int width, height;

    public int treasureWidth, treasureHeight;

    public float blockSize = 1.0f;

    public bool reverseX;

    protected Tile[,] tiles;

    private bool[,] tilesChecked;

    private Vector3 minCoord, maxCoord, minTreasure, maxTreasure;

    void Awake()
    {
        tiles = new Tile[width, height];
        tilesChecked = new bool[width, height];

        for (int j = 0; j < width; j++)
        {
            for (int k = 0; k < height; k++)
            {
                tiles[j, k] = new Tile(this, j, k, blockSize);
                tilesChecked[j, k] = false;
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        Vector3 pos = transform.position;

        if (!reverseX)
        {
            minCoord = pos;
            minTreasure = pos;

            maxCoord = new Vector3(pos.x + width * blockSize, pos.y + height * blockSize);
            maxTreasure = new Vector3(pos.x + treasureWidth * blockSize, pos.y + treasureHeight * blockSize);
        }
        else
        {
            minCoord = new Vector3(pos.x - width * blockSize, pos.y);
            minTreasure = new Vector3(pos.x - treasureWidth * blockSize, pos.y);

            maxCoord = new Vector3(pos.x, pos.y + height * blockSize);
            maxTreasure = new Vector3(pos.x, pos.y + treasureHeight * blockSize);
        }
    }

    public bool IsInside(Vector3 position)
    {
        return minCoord.x < position.x && minCoord.y < position.y &&
               maxCoord.x > position.x && maxCoord.y > position.y;
    }

    public bool CanPlace(Vector3 position)
    {
        return minCoord.x < position.x && minCoord.y < position.y &&
               maxCoord.x > position.x && maxCoord.y > position.y &&
               !(minTreasure.x < position.x && minTreasure.y < position.y &&
               maxTreasure.x > position.x && maxTreasure.y > position.y);
    }

    public int GetXCoord(float xPos)
    {
        if (!reverseX)
        {
            return Mathf.FloorToInt((xPos - transform.position.x) / blockSize);
        }
        else
        {
            return Mathf.FloorToInt((transform.position.x - xPos) / blockSize);
        }
    }

    public int GetYCoord(float yPos)
    {
        return Mathf.FloorToInt((yPos - transform.position.y) / blockSize);
    }

    public Vector3 CenterOn(Vector3 position)
    {
        return tiles[GetXCoord(position.x), GetYCoord(position.y)].GetCenter();
    }

    public void LoadBlock(int x, int y, GameObject block)
    {
        GameObject placed = Instantiate(block, tiles[x, y].GetCenter(), new Quaternion()) as GameObject;

        Breakable breakable = placed.GetComponent<Breakable>();
        tiles[x, y].SetBlock(breakable);
    }

    public void RemoveBlock(GameObject block)
    {
        SaveFile.Instance().RemoveTile(tiles[GetXCoord(block.transform.position.x), GetYCoord(block.transform.position.y)]);
    }

    public void DetachAdjacentBlocks(int x, int y)
    {
        //if (x > 0)
        //{
        //    if (IsGrounded(x - 1, y))
        //    {

        //    }
        //}
    }

    //bool IsGrounded(int x, int y)
    //{
    //    ResetTilesChecked();

    //    if (y == 0 && !tiles[x, y].Empty())
    //    {
    //        return true;
    //    }

    //    if (y > 0)
    //    {
    //        InternalIsGrounded
    //    }
    //}

    //bool InternalIsGrounded(int x, int y)
    //{

    //}

    void ResetTilesChecked()
    {
        for (int j = 0; j < width; j++)
        {
            for (int k = 0; k < height; k++)
            {
                tilesChecked[j, k] = false;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 mnc, mxc, mnt, mxt;

        Vector3 pos = transform.position;

        if (!reverseX)
        {
            mnc = pos;
            mnt = pos;

            mxc = new Vector3(pos.x + width * blockSize, pos.y + height * blockSize);
            mxt = new Vector3(pos.x + treasureWidth * blockSize, pos.y + treasureHeight * blockSize);
        }
        else
        {
            mnc = new Vector3(pos.x - width * blockSize, pos.y);
            mnt = new Vector3(pos.x - treasureWidth * blockSize, pos.y);

            mxc = new Vector3(pos.x, pos.y + height * blockSize);
            mxt = new Vector3(pos.x, pos.y + treasureHeight * blockSize);
        }

        Gizmos.color = Color.green;

        Gizmos.DrawLine(mnc, new Vector3(mnc.x, mxc.y));
        Gizmos.DrawLine(mxc, new Vector3(mnc.x, mxc.y));
        Gizmos.DrawLine(mnc, new Vector3(mxc.x, mnc.y));
        Gizmos.DrawLine(mxc, new Vector3(mxc.x, mnc.y));

        Gizmos.color = Color.yellow;

        Gizmos.DrawCube((mnt + mxt) / 2f, (mxt - mnt));
    }
}
