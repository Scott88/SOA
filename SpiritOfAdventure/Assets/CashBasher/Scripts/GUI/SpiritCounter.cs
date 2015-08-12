using UnityEngine;
using System.Collections;

public class SpiritCounter : MonoBehaviour {

    public TextMesh display;
    public SpiritType type;

    private int spiritCount = 0;

    public void Add()
    {
        spiritCount++;
        display.text = "x" + spiritCount.ToString();

        SaveFile.Instance().SetSpiritCount(type, spiritCount);
    }

    public void Add(int count)
    {
        spiritCount += count;
        display.text = "x" + spiritCount.ToString();

        SaveFile.Instance().SetSpiritCount(type, spiritCount);
    }

    public void Remove()
    {
        spiritCount--;
        display.text = "x" + spiritCount.ToString();

        if (GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().Play();
        }

        SaveFile.Instance().SetSpiritCount(type, spiritCount);
    }

    public bool Empty()
    {
        return spiritCount == 0;
    }
}
