using UnityEngine;
using System.Collections;

public class ActionGiveStars : SOAAction
{
    public int stars;

    public Camera objectCamera, inventoryCamera;

    public GameObject starSpawner;

    protected override void Activate()
    {
        GameObject spawner = Instantiate(starSpawner, transform.position, Quaternion.identity) as GameObject;

        spawner.GetComponent<StarSpawner>().Go(stars, inventoryCamera, objectCamera, false);
    }
}
