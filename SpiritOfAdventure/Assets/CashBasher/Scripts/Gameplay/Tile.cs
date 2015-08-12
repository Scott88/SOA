using UnityEngine;
using System.Collections;

public class Tile 
{
    int x, y;

    float blockSize;

    Breakable block;

    TileSet parent;

    public Tile(TileSet p, int xCoord, int yCoord, float size)
    {
        parent = p;
        x = xCoord;
        y = yCoord;
        block = null;
        blockSize = size;
    }

    public Tile(TileSet p, Breakable b, int xCoord, int yCoord, float size)
    {
        parent = p;
        block = b;
        x = xCoord;
        y = yCoord;
        blockSize = size;
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

    public void InsertSpirit(int spiritType)
    {
        block.GetComponent<NetworkView>().RPC("SetSpirit", RPCMode.All, spiritType);
    }

    public SpiritType GetDebuff()
    {
        if (block)
        {
            if(block.IsDying())
            {
                return SpiritType.ST_NULL;
            }
            else
            {
                return block.GetStatusEffect();
            }
        }
        else
        {
            return SpiritType.ST_NULL;
        }
    }

    public bool IsDebuffedBy(SpiritType type)
    {
        if (block)
        {
            return block.IsDebuffedBy(type);
        }
        else
        {
            return false;
        }
    }

    public bool IsHealedBy(SpiritType type)
    {
        if (block)
        {
            return block.IsHealedBy(type);
        }
        else
        {
            return false;
        }
    }

    public void Debuff(SpiritType type)
    {
        block.SetStatusEffect(type, false);
    }

    public void Heal()
    {
        block.SetStatusEffect(SpiritType.ST_NULL, true);
    }

    public void Tick()
    {
        if (block)
        {
            block.Tick();
        }
    }

    public void Apply()
    {
        if (block)
        {
            block.Apply();
        }
    }

    public Vector3 GetCenter()
    {
        Vector3 pos = parent.transform.position;

        if (!parent.reverseX)
        {
            return new Vector3(pos.x + (x * blockSize) + blockSize / 2f, pos.y + (y * blockSize) + blockSize / 2f);
        }
        else
        {
            return new Vector3(pos.x - (x * blockSize) - blockSize / 2f, pos.y + (y * blockSize) + blockSize / 2f);
        }
    }

    public void SetBlock(Breakable b)
    {
        block = b;
        block.SetTile(this);
    }

    public void DestroyBlock()
    {
        GameObject.Destroy(block.gameObject);
    }

    public bool Empty()
    {
        return block == null;
    }

    public void TransferTo(Tile tile, float acceleration)
    {
        block.FallTo(tile.GetCenter(), acceleration, y - tile.GetY() >= 2);

        tile.block = block;
        block = null;
    }
}
