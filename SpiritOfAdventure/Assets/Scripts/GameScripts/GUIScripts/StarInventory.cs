using UnityEngine;
using System.Collections;

public class StarInventory : MonoBehaviour
{
    public TextMesh starDisplay;

    public StarInventory childInventory;

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

        if (childInventory)
        {
            childInventory.stars += count;
            childInventory.starDisplay.text = "x" + stars;
        }
    }

    public void Remove(int count)
    {
        stars -= count;
        starDisplay.text = "x" + stars;

        SaveFile.Instance().ModifyStars(-count);

        if (childInventory)
        {
            childInventory.stars += count;
            childInventory.starDisplay.text = "x" + stars;
        }
    }

    public int GetStars()
    {
        return stars;
    }
}
