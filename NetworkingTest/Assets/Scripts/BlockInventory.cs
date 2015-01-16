using UnityEngine;
using System.Collections;

public class BlockInventory : MonoBehaviour
{
    public TextMesh blockCounter;
    public GameObject selectionIndicator;

    private int blocks;

    private bool selected = false;

    // Use this for initialization
    void Start()
    {
        if (PlayerPrefs.HasKey("Blocks"))
        {
            blocks = PlayerPrefs.GetInt("Blocks");
        }
        else
        {
            blocks = 4;
            PlayerPrefs.SetInt("Blocks", blocks);
        }

        blockCounter.text = "x" + blocks.ToString();
    }

    public void Select()
    {
        if (blocks > 0)
        {
            blocks--;
            blockCounter.text = "x" + blocks.ToString();
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
                blocks++;
                blockCounter.text = "x" + blocks.ToString();
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
        PlayerPrefs.SetInt("Blocks", blocks > 3 ? blocks : 3);
    }
}
