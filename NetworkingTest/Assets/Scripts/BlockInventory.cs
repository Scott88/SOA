using UnityEngine;
using System.Collections;

public class BlockInventory : MonoBehaviour
{
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
    }

    public void Select()
    {
        blocks--;
        selected = true;
    }

    public void Deselect()
    {
        blocks++;
        selected = false;
    }

    public bool IsSelected()
    {
        return selected;
    }

    public void Save()
    {
        PlayerPrefs.SetInt("Blocks", blocks);
    }
}
