using UnityEngine;
using System.Collections;

public class BlockCounter : MonoBehaviour
{
    public TextMesh display;

    private int blockCount = 0;

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
}
