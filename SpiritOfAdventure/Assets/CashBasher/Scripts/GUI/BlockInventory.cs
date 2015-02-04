using UnityEngine;
using System.Collections;

public class BlockInventory : MonoBehaviour
{
    public GameObject block;
    public string blockName;

    public TextMesh blockCounter;
    public GameObject selectionIndicator;

    public GameObject spawnIndicator;

    private int blockCount;

    private bool selected = false;

    // Use this for initialization
    void Start()
    {
        if (PlayerPrefs.HasKey(blockName))
        {
            //blockCount = PlayerPrefs.GetInt(blockName);
            blockCount = 10;
        }
        else
        {
            blockCount = 10;
            PlayerPrefs.SetInt(blockName, blockCount);
        }

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
            blockCount--;
            blockCounter.text = "x" + blockCount.ToString();
            selected = true;
            selectionIndicator.SetActive(true);
        }
    }

    public void Deselect(bool blockPlaced)
    {
        if (selected)
        {
            if (!blockPlaced)
            {
                ReturnBlock();
            }

            selected = false;
            selectionIndicator.SetActive(false);
        }
    }

    public void ReturnBlock()
    {
        blockCount++;
        blockCounter.text = "x" + blockCount.ToString();
    }

    public bool IsSelected()
    {
        return selected;
    }

    public void Save()
    {
        PlayerPrefs.SetInt(blockName, blockCount > 3 ? blockCount : 3);
    }
}
