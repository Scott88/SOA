using UnityEngine;
using System.Collections;

public class StarInventory : MonoBehaviour
{
    public TextMesh starDisplay;

    private int stars;

    void Start()
    {
        stars = SaveFile.Instance().GetStars();
        starDisplay.text = "x" + stars;
    }

    public void Add(int count)
    {
        stars += count;
        starDisplay.text = "x" + stars;

        SaveFile.Instance().ModifyStars(count);
    }

    public void Remove(int count)
    {
        stars -= count;
        starDisplay.text = "x" + stars;

        SaveFile.Instance().ModifyStars(-count);
    }

    public int GetStars()
    {
        return stars;
    }
}
