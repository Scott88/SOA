using UnityEngine;
using System.Collections;

public class BlockCounter : MonoBehaviour
{
    public TextMesh display;

    private int blockCount = 0;

    private int startBlockCount;

    public void Add()
    {
        blockCount++;
        display.text = "x" + blockCount.ToString();
    }

    public void Remove()
    {
        blockCount--;
        display.text = "x" + blockCount.ToString();
    }

    public int RemoveAll()
    {
        int originalBlockCount = blockCount;

        blockCount = 0;
        display.text = "x0";

        return originalBlockCount;
    }

    public int GetBlocks()
    {
        return blockCount;
    }

    public void Finalize()
    {
        startBlockCount = blockCount;
    }
}
