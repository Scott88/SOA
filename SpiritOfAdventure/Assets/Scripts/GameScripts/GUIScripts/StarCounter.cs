using UnityEngine;
using System.Collections;

public class StarCounter : MonoBehaviour
{
    public TextMesh starDisplay;

    public GameObject starSpawner;

    public Camera myCam, inventoryCam;

    private int stars;

    private bool transferStarted = false;

    void Start()
    {
        stars = 0;       
        starDisplay.text = "x" + stars;
    }

    public void Add(int count)
    {
        stars += count;
        starDisplay.text = "x" + stars;
    }

    public void Remove(int count)
    {
        stars -= count;
        starDisplay.text = "x" + stars;
    }

    public int GetStars()
    {
        return stars;
    }

    public void TransferStars(float delay = 0.1f)
    {
        GameObject s = Instantiate(starSpawner, transform.position, Quaternion.identity) as GameObject;

        s.GetComponent<StarSpawner>().Go(stars, inventoryCam, myCam, true, delay);

        transferStarted = true;
    }

    public bool TransferStarted() { return transferStarted; }

    public float GetStarGiveDuration()
    {
        return 0.85f + (float)stars * 0.05f;
    }

    public void Clear()
    {
        stars = 0;
        starDisplay.text = "x0";
    }
	
}
