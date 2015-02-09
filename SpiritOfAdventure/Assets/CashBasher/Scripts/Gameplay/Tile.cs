using UnityEngine;
using System.Collections;

public class Tile 
{
    int x, y;

    Breakable block;

    TileSet parent;

    public Tile(TileSet p, int xCoord, int yCoord)
    {
        parent = p;
        x = xCoord;
        y = yCoord;
        block = null;
    }

    public Tile(TileSet p, Breakable b, int xCoord, int yCoord)
    {
        parent = p;
        block = b;
        x = xCoord;
        y = yCoord;
    }

    public int GetX() { return x; }
    public int GetY() { return y; }

    public BlockType GetBlockType()
    {
        if (block == null)
        {
            return BlockType.BT_NULL;
        }
        else
        {
            return block.type;
        }
    }

    public Vector3 GetCenter()
    {
        Vector3 pos = parent.transform.position;

        if (!parent.reverseX)
        {
            return new Vector3(pos.x + x + 0.5f, pos.y + y + 0.5f);
        }
        else
        {
            return new Vector3(pos.x - x - 0.5f, pos.y + y + 0.5f);
        }
    }

    public void SetBlock(Breakable b)
    {
        block = b;
    }

    public bool Empty()
    {
        return block == null;
    }
}
