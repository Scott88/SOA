using UnityEngine;
using System.Collections;

public class SpiritInventory : MonoBehaviour
{
    public SpiritType type;

    public string spiritName;

    public TextMesh spiritCounter;

    private int spiritCount;

    private bool selected = false;

    // Use this for initialization
    void Start()
    {
        if (PlayerPrefs.HasKey(spiritName))
        {
            //blockCount = PlayerPrefs.GetInt(blockName);
            spiritCount = 10;
        }
        else
        {
            spiritCount = 10;
            PlayerPrefs.SetInt(spiritName, spiritCount);
        }

        spiritCounter.text = "x" + spiritCount.ToString();
    }

    public bool Empty()
    {
        return spiritCount == 0;
    }

    public void RemoveSpirit()
    {
        if (spiritCount > 0)
        {
            spiritCount--;
            spiritCounter.text = "x" + spiritCount.ToString();
        }
    }

    public void AddSpirit()
    {
        spiritCount++;
        spiritCounter.text = "x" + spiritCount.ToString();
    }


    public void Save()
    {
        PlayerPrefs.SetInt(spiritName, spiritCount);
    }
}
