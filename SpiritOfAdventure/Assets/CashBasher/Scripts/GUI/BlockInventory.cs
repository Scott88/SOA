using UnityEngine;
using System.Collections;

public class BlockInventory : MonoBehaviour
{
    public GameObject block;

    public TextMesh blockCounter;
    public GameObject selectionIndicator;

    public GameObject spawnIndicator;

    public BlockType type;

    public BlockInventory childInventory;

    public bool selectable = true;

    private int blockCount;

    private bool selected = false;

    void Awake()
    {
        blockCount = SaveFile.Instance().GetBlockInventory(type);

        blockCounter.text = "x" + blockCount.ToString();
    } 

    public bool Empty()
    {
        return blockCount == 0;
    }

    public GameObject GetBlock()
    {
        return block;
    }

    public void Select()
    {
        if (blockCount > 0)
        {
            selected = true;
            selectionIndicator.SetActive(true);
        }
    }

    public void Deselect()
    {
        if (selected)
        {
            selected = false;
            selectionIndicator.SetActive(false);
        }
    }

    public void TakeBlock()
    {
        blockCount--;

        UpdateDisplay();
    }

    public void ReturnBlock()
    {
        blockCount++;

        UpdateDisplay();
    }

    public void GiveBlocks(int count)
    {
        blockCount += count;

        UpdateDisplay();

        if (childInventory)
        {
            childInventory.GiveBlocks(count);
        }
    }

    void UpdateDisplay()
    {
        blockCounter.text = "x" + blockCount.ToString();
    }

    public bool IsSelected()
    {
        return selected;
    }
}
