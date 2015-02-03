using UnityEngine;
using System.Collections;

public class BlockInventory : MonoBehaviour
{
    public GameObject block;
    public string blockName;

    public TextMesh blockCounter;
    public GameObject selectionIndicator;

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
                blockCount++;
                blockCounter.text = "x" + blockCount.ToString();
            }

            selected = false;
            selectionIndicator.SetActive(false);
        }
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
