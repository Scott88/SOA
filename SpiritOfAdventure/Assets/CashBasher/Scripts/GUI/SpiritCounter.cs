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
    }

    public void Remove()
    {
        spiritCount--;
        display.text = "x" + spiritCount.ToString();
    }

    public bool Empty()
    {
        return spiritCount == 0;
    }
}
