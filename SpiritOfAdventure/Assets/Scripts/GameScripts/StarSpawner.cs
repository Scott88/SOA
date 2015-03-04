using UnityEngine;
using System.Collections;

public class StarSpawner : MonoBehaviour
{
    public GameObject earnedStar;

    public bool goToInventory;

    public void Go(int numStars, Camera inventoryCam, Camera objectCam, bool takeFromCounter, float delay = 0.1f, float gapFactor = 0.75f)
    {
        if (goToInventory)
        {
            FindObjectOfType<StarInventory>().Add(numStars);
        }

        StartCoroutine(CollectStars(numStars, inventoryCam, objectCam, takeFromCounter, delay, gapFactor));
    }

    IEnumerator CollectStars(int numStars, Camera inventoryCam, Camera objectCam, bool takeFromCounter, float delay, float gapFactor)
    {
        float delayIncrement = (gapFactor + (float)numStars * 0.05f) / (float)numStars;

        StarCounter counter = FindObjectOfType<StarCounter>();

        for (int j = 0; j < numStars; j++)
        {
            Vector3 spawnPoint;

            if (objectCam != inventoryCam)
            {
                spawnPoint = inventoryCam.ScreenToWorldPoint(objectCam.WorldToScreenPoint(transform.position));
            }
            else
            {
                spawnPoint = transform.position;
            }

            if (goToInventory && takeFromCounter)
            {
                counter.Remove(1);
            }

            GameObject star = Instantiate(earnedStar, spawnPoint, Quaternion.identity) as GameObject;

            EarnedStar starGiver = star.GetComponent<EarnedStar>();

            starGiver.delay = delay;
            starGiver.goToInventory = goToInventory;
            starGiver.Go();

            yield return new WaitForSeconds(delayIncrement);
        }

        Destroy(gameObject);
    }
}
