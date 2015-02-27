#define USE_SIS_STARS

using UnityEngine;
using System.Collections;
using SIS;

public class StarInventory : MonoBehaviour
{
    public TextMesh starDisplay;

    public StarInventory childInventory;

    public DBManager database;

    private int stars;

    void Start()
    {
        if (database)
        {
            database.Init();
        }

#if USE_SIS_STARS
        stars = DBManager.GetFunds("stars");       
#else
        stars = SaveFile.Instance().GetStars();
#endif
        starDisplay.text = "x" + stars;
    }

    public void Add(int count)
    {
        stars += count;
        starDisplay.text = "x" + stars;

#if USE_SIS_STARS
        DBManager.IncreaseFunds("stars", 1);
#else
        SaveFile.Instance().ModifyStars(count);
#endif

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

#if USE_SIS_STARS
        Debug.LogError("Cannot remove SIS stars without an SIS purchase!");
#else
        SaveFile.Instance().ModifyStars(-count);
#endif

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
