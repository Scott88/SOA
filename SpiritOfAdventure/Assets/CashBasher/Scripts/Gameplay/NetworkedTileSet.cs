using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkedTileSet : TileSet
{
    public GameObject woodBlock, stoneBlock, metalBlock;

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
        tiles[x, y].SetBlock(breakable);
    }
}
