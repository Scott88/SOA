using UnityEngine;
using System.Collections;

public class CashBasherSpiritGUI : MonoBehaviour
{
    public SpiritType type;

    public CashBasherSpirit spirit;

    public TextMesh display;

    private int count;

    void Start()
    {
        count = SaveFile.Instance().GetSpiritCount(type);
        display.text = "x" + count;
    }

    public bool Empty()
    {
        return count == 0;
    }

    public void Add()
    {
        count++;
        display.text = "x" + count;

        SaveFile.Instance().ModifySpiritInventory(type, 1);
    }

    public void Remove()
    {
        count--;
        display.text = "x" + count;

        SaveFile.Instance().ModifySpiritInventory(type, -1);
    }  
}
