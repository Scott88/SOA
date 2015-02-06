using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkedTileSet : TileSet
{
    public BlockCounter woodCounter, stoneCounter, metalCounter;

    public GameObject woodBlock, stoneBlock, metalBlock;

    public void Load()
    {
        IEnumerator<SaveFile.TileInfo> tileList = SaveFile.Instance().GetTileList();

        while (tileList.MoveNext())
        {
            switch (tileList.Current.type)
            {
                case BlockType.BT_WOOD:
                    LoadBlock(tileList.Current.x, tileList.Current.y, woodBlock, woodCounter);
                    break;
                case BlockType.BT_STONE:
                    LoadBlock(tileList.Current.x, tileList.Current.y, stoneBlock, stoneCounter);
                    break;
                case BlockType.BT_METAL:
                    LoadBlock(tileList.Current.x, tileList.Current.y, metalBlock, metalCounter);
                    break;
            }
        }
    }

    void LoadBlock(int x, int y, GameObject block, BlockCounter counter)
    {
        LoadNetworkedBlock(x, y, block);

        counter.Add();       
    }

    public void LoadNetworkedBlock(int x, int y, GameObject block)
    {
        GameObject placed = Network.Instantiate(block, tiles[x, y].GetCenter(), new Quaternion(), 0) as GameObject;

        Breakable breakable = placed.GetComponent<Breakable>();
        tiles[x, y].SetBlock(breakable);
    }
}
