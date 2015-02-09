using UnityEngine;
using System.Collections;

public class EditorTileSet : TileSet
{
    public BlockType PlaceAndSaveBlock(Vector3 position, GameObject block)
    {
        int x = GetXCoord(position.x);
        int y = GetYCoord(position.y);

        GameObject placed = Instantiate(block, tiles[x, y].GetCenter(), new Quaternion()) as GameObject;

        Breakable breakable = placed.GetComponent<Breakable>();
        tiles[x, y].SetBlock(breakable);

        SaveFile.Instance().AddTile(tiles[x, y]);

        return breakable.type;
    }
}
