using UnityEngine;
using System.Collections;

public class ActionGiveStars : SOAAction
{
    public int stars;

    public Camera playerCamera, guiCamera;

    public GameObject starSpawner;

    protected override void Activate()
    {
        GameObject spawner = Instantiate(starSpawner, transform.position, Quaternion.identity) as GameObject;

        spawner.GetComponent<StarSpawner>().Go(stars, guiCamera, playerCamera);
    }
}
