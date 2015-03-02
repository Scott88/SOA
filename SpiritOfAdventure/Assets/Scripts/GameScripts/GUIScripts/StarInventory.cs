#define USE_SIS_STARS

using UnityEngine;
using System.Collections;
using SIS;

public class StarInventory : MonoBehaviour
{
    public TextMesh starDisplay;

    public StarInventory childInventory;

    private int stars;

    void Start()
    {
        MatchDisplay();
    }

    public void Add(int count)
    {
#if USE_SIS_STARS
        if (DBManager.GetInstance())
        {
            DBManager.IncreaseFunds("stars", count);
        }
#else
        SaveFile.Instance().ModifyStars(count);
#endif    
    }

    public void DisplayAdd(int count)
    {
        stars += count;
        starDisplay.text = "x" + stars;

        if (childInventory)
        {
            childInventory.stars += count;
            childInventory.starDisplay.text = "x" + stars;
        }
    }

    public void Remove(int count)
    {
#if USE_SIS_STARS
        Debug.LogError("Cannot remove SIS stars without an SIS purchase!");
#else
        SaveFile.Instance().ModifyStars(-count);
#endif

        DisplayRemove(count);
    }

    public void DisplayRemove(int count)
    {
        stars -= count;
        starDisplay.text = "x" + stars;

        if (childInventory)
        {
            childInventory.stars += count;
            childInventory.starDisplay.text = "x" + stars;
        }
    }

    public void MatchDisplay()
    {
#if USE_SIS_STARS
        if (DBManager.GetInstance())
        {
            stars = DBManager.GetFunds("stars");
        }
#else
        stars = SaveFile.Instance().GetStars();
#endif
        starDisplay.text = "x" + stars;
    }

    public int GetStars()
    {
        return stars;
    }
}
