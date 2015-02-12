using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkedTileSet : TileSet
{
    public GameObject woodBlock, stoneBlock, metalBlock;

    public SpreadChances vineChances, waterChances, fireChances;

    public CashBasherManager manager;

    public void Load()
    {
        IEnumerator<SaveFile.TileInfo> tileList = SaveFile.Instance().GetTileList();

        while (tileList.MoveNext())
        {
            switch (tileList.Current.type)
            {
                case BlockType.BT_WOOD:
                    LoadNetworkedBlock(tileList.Current.x, tileList.Current.y, woodBlock);
                    break;
                case BlockType.BT_STONE:
                    LoadNetworkedBlock(tileList.Current.x, tileList.Current.y, stoneBlock);
                    break;
                case BlockType.BT_METAL:
                    LoadNetworkedBlock(tileList.Current.x, tileList.Current.y, metalBlock);
                    break;
            }
        }
    }

    public void LoadNetworkedBlock(int x, int y, GameObject block)
    {
        GameObject placed = Network.Instantiate(block, tiles[x, y].GetCenter(), new Quaternion(), 0) as GameObject;

        Breakable breakable = placed.GetComponent<Breakable>();

        if (breakable)
        {
            tiles[x, y].SetBlock(breakable);
        }
    }

    public void Spread()
    {
        for (int j = 0; j < width; j++)
        {
            for (int k = 0; k < height; k++)
            {
                TrySpread(tiles[j, k]);
            }
        }
    }

    void TrySpread(Tile tile)
    {
        SpiritType debuff = tile.GetDebuff();

        switch (debuff)
        {
            case SpiritType.ST_GREEN:
                SpreadFromTile(tile, vineChances, SpiritType.ST_GREEN);
                break;
            case SpiritType.ST_BLUE:
                SpreadFromTile(tile, waterChances, SpiritType.ST_BLUE);
                break;
            case SpiritType.ST_RED:
                SpreadFromTile(tile, fireChances, SpiritType.ST_RED);
                break;
        }
    }

    void SpreadFromTile(Tile tile, SpreadChances chances, SpiritType type)
    {
        int x = tile.GetX(), y = tile.GetY();

        if (x - 1 >= 0)
        {
            SpreadToTile(tiles[x - 1, y], chances.spreadSides, type);

            if (y - 1 >= 0)
            {
                SpreadToTile(tiles[x - 1, y - 1], chances.spreadDiagonalDown, type);
            }

            if (y + 1 < height)
            {
                SpreadToTile(tiles[x - 1, y + 1], chances.spreadDiagonalUp, type);
            }
        }

        if (y - 1 >= 0)
        {
            SpreadToTile(tiles[x, y - 1], chances.spreadDown, type);
        }

        if (y + 1 < height)
        {
            SpreadToTile(tiles[x, y + 1], chances.spreadUp, type);
        }

        if (x + 1 < width)
        {
            SpreadToTile(tiles[x + 1, y], chances.spreadSides, type);

            if (y - 1 >= 0)
            {
                SpreadToTile(tiles[x + 1, y - 1], chances.spreadDiagonalDown, type);
            }

            if (y + 1 < height)
            {
                SpreadToTile(tiles[x + 1, y + 1], chances.spreadDiagonalUp, type);
            }
        }
    }

    void SpreadToTile(Tile tile, float chance, SpiritType type)
    {
        if (tile.IsDebuffedBy(type) && tile.GetDebuff() == SpiritType.ST_NULL)
        {
            if (Random.value <= chance)
            {
                tile.Debuff(type);
            }
        }
    }

    void Tick()
    {
        for (int j = 0; j < width; j++)
        {
            for (int k = 0; k < height; k++)
            {
                tiles[j, k].Tick();
            }
        }
    }

    void Apply()
    {
        for (int j = 0; j < width; j++)
        {
            for (int k = 0; k < height; k++)
            {
                tiles[j, k].Apply();
            }
        }
    }

    public void HealFrom(Vector3 position)
    {

    }

    public bool HasStatusEffects()
    {
        for (int j = 0; j < width; j++)
        {
            for (int k = 0; k < height; k++)
            {
                if (tiles[j, k].GetDebuff() != SpiritType.ST_NULL)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void TickDebuffs()
    {
        Spread();
        Tick();
        Apply();
    }
}
