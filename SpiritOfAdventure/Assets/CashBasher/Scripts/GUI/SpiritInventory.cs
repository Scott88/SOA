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
    void Awake()
    {
        spiritCount = SaveFile.Instance().GetSpiritInventory(type);
        spiritCounter.text = "x" + spiritCount.ToString(); 
    }

    public bool Empty()
    {
        return spiritCount == 0;
    }

    public int RemoveSpirits(int count)
    {
        if (count > spiritCount)
        {
            int total = spiritCount;
            spiritCount = 0;

            UpdateDisplay();

            return total;
        }
        else
        {
            spiritCount -= count;

            UpdateDisplay();

            return count;
        }   
    }

    public void RemoveSpirit()
    {
        if (spiritCount > 0)
        {
            spiritCount--;

            UpdateDisplay();

            if (audio)
            {
                audio.Play();
            }
        }
    }

    public void AddSpirit()
    {
        spiritCount++;

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        spiritCounter.text = "x" + spiritCount.ToString();
    }
}
