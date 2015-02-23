using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkedTileSet : TileSet
{
    public float fallAcceleration;

    public GameObject woodBlock, stoneBlock, metalBlock;

    public SpreadChances vineChances, waterChances, fireChances;

    public CashBasherManager manager;

    private bool filledOut = false;

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

        filledOut = true;
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
    
    public void GenerateBlockSpirits(float spiritChance, int minSpirits)
    {
        List<Tile> blockList = GetTilesWithBlocks();

        int spiritsPlaced = 0;

        for(int j = 0; j < blockList.Count; j++)
        {
            if (Random.value < spiritChance)
            {
                InsertSpiritIntoTile(blockList[j]);
                blockList.RemoveAt(j);
                spiritsPlaced++;
            }
        }

        while (spiritsPlaced < minSpirits && minSpirits - spiritsPlaced < blockList.Count)
        {
            int index = Random.Range(0, blockList.Count);
            InsertSpiritIntoTile(blockList[index]);
            blockList.RemoveAt(index);
            spiritsPlaced++;
        }
    }

    List<Tile> GetTilesWithBlocks()
    {
        List<Tile> blockTileList = new List<Tile>();

        for (int j = 0; j < width; j++)
        {
            for (int k = 0; k < height; k++)
            {
                if (tiles[j, k].GetBlockType() != BlockType.BT_NULL)
                {
                    blockTileList.Add(tiles[j, k]);
                }
            }
        }

        return blockTileList;
    }

    void InsertSpiritIntoTile(Tile tile)
    {
        tile.InsertSpirit(Random.Range(1, 4));
    }

    void Spread()
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

    public void HealFrom(Vector3 position, SpiritType type)
    {
        if (!filledOut)
        {
            return;
        }

        int x = GetXCoord(position.x), y = GetYCoord(position.y);

        Heal(tiles[x, y], type);

        if (x + 1 < width)
        {
            Heal(tiles[x + 1, y], type);

            if (y + 1 < height)
            {
                Heal(tiles[x + 1, y + 1], type);
            }

            if (y - 1 > 0)
            {
                Heal(tiles[x + 1, y - 1], type);
            }

            if (x + 2 < width)
            {
                Heal(tiles[x + 2, y], type);
            }
        }

        if (x - 1 > 0)
        {
            Heal(tiles[x - 1, y], type);

            if (y + 1 < height)
            {
                Heal(tiles[x - 1, y + 1], type);
            }

            if (y - 1 > 0)
            {
                Heal(tiles[x - 1, y - 1], type);
            }

            if (x - 2 > 0)
            {
                Heal(tiles[x - 2, y], type);
            }
        }

        if (y + 1 < height)
        {
            Heal(tiles[x, y + 1], type);
        }

        if (y - 1 > 0)
        {
            Heal(tiles[x, y - 1], type);
        }

        if (y + 2 < height)
        {
            Heal(tiles[x, y + 2], type);
        }

        if (y - 2 > 0)
        {
            Heal(tiles[x, y - 2], type);
        }
    }

    void Heal(Tile tile, SpiritType type)
    {
        if (tile.IsHealedBy(type))
        {
            tile.Heal();
        }
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

    //public float ApplyGravity()
    //{
    //    int longestFall = 0;
        
    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {
    //            int nextYLevel = GetNextTileY(x, y);

    //            if (nextYLevel != -1)
    //            {
    //                if (y - nextYLevel > longestFall)
    //                {
    //                    longestFall = y - nextYLevel;
    //                }

    //                tiles[x, y].TransferTo(tiles[x, nextYLevel], fallAcceleration);
    //            }
    //        }
    //    }

    //    float distance = longestFall * blockSize;
    //}

    //int GetNextTileY(int x, int y)
    //{
    //    if (y == 0)
    //    {
    //        return -1;
    //    }
    //    else
    //    {
    //        if (BlockIsSupported(x, y - 1, 2, true, true))
    //        {
    //            return -1;
    //        }
    //    }

    //    if (x > 0)
    //    {
    //        if (BlockIsSupported(x - 1, y, 1, true, false))
    //        {
    //            return -1;
    //        }
    //    }

    //    if (x < width - 1)
    //    {
    //        if (BlockIsSupported(x + 1, y, 1, false, true))
    //        {
    //            return -1;
    //        }
    //    }

    //    for (int k = y - 1; k >= 0; k--)
    //    {
    //        if (BlockIsSupported(x, k, 0, false, false))
    //        {
    //            return k + 1;
    //        }
    //    }

    //    return 0;
    //}

    //bool BlockIsSupported(int x, int y, int overhang, bool checkLeft, bool checkRight)
    //{
    //    if (tiles[x, y].Empty())
    //    {
    //        return false;
    //    }

    //    if (y == 0)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        if (BlockIsSupported(x, y - 1, overhang, true, true))
    //        {
    //            return true;
    //        }
    //    }

    //    if (overhang > 0)
    //    {
    //        if (checkLeft && x > 0)
    //        {
    //            if(BlockIsSupported(x - 1, y, overhang - 1, true, false))
    //            {
    //                return true;
    //            }
    //        }

    //        if (checkRight && x < width - 1)
    //        {
    //            if (BlockIsSupported(x + 1, y, overhang - 1, false, true))
    //            {
    //                return true;
    //            }
    //        }
    //    }

    //    return false;
    //}

}
