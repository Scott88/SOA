using UnityEngine;
using System.Collections;

public class StarSpawner : MonoBehaviour
{
    public GameObject earnedStar;

    public void Go(int numStars, Camera guiCamera, Camera playerCamera)
    {
        StartCoroutine(CollectStars(numStars, guiCamera, playerCamera));
    }

    IEnumerator CollectStars(int numStars, Camera guiCamera, Camera playerCamera)
    {
        float delay = 0.1f;
        float delayIncrement = (0.75f + (float)numStars * 0.05f) / (float)numStars;

        for (int j = 0; j < numStars; j++)
        {
            Vector3 spawnPoint = guiCamera.ScreenToWorldPoint(playerCamera.WorldToScreenPoint(transform.position));

            GameObject star = Instantiate(earnedStar, spawnPoint, Quaternion.identity) as GameObject;

            EarnedStar starGiver = star.GetComponent<EarnedStar>();

            starGiver.delay = delay;
            starGiver.Go();

            yield return new WaitForSeconds(delayIncrement);
        }

        Destroy(gameObject);
    }
}
