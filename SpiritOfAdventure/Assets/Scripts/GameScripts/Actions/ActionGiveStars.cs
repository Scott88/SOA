using UnityEngine;
using System.Collections;

public class ActionGiveStars : SOAAction
{
    public int stars;

    public Camera playerCamera, guiCamera;

    public GameObject starPrefab;

    protected override void Activate()
    {
        Vector3 spawnPos = guiCamera.ScreenToWorldPoint(playerCamera.WorldToScreenPoint(transform.position));

        float delay = 0.1f;
        float delayIncrement = (0.75f + (float)stars * 0.05f) / (float)stars;

        for (int j = 0; j < stars; j++)
        {
            GameObject star = Instantiate(starPrefab, spawnPos, Quaternion.identity) as GameObject;

            EarnedStar starGiver = star.GetComponent<EarnedStar>();

            starGiver.delay = delay;
            starGiver.Go();

            delay += delayIncrement;
        }
    }
}
