using UnityEngine;
using System.Collections;

public class EarnedStar : MonoBehaviour
{
    public float delay { get; set; }

    public bool goToInventory { get; set; }

    private StarInventory inventory;
    private StarCounter counter;

    private Vector3 velocityRef;

    private bool starAdded = false;

    void Start()
    {
        if (goToInventory)
        {
            inventory = FindObjectOfType<StarInventory>();
        }
        else
        {
            counter = FindObjectOfType<StarCounter>();
        }
    }

    public void Go()
    {
        if (goToInventory)
        {
            StartCoroutine(GoToInventory());
        }
        else
        {
            StartCoroutine(GoToCounter());
        }
    }

    IEnumerator GoToInventory()
    {
        yield return new WaitForSeconds(delay);

        while ((inventory.transform.position - transform.position).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, inventory.transform.position, ref velocityRef, 0.25f);
            yield return 0;
        }

        inventory.DisplayAdd(1);

        starAdded = true;

        Destroy(gameObject);
    }

    IEnumerator GoToCounter()
    {
        yield return new WaitForSeconds(delay);

        while ((counter.transform.position - transform.position).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, counter.transform.position, ref velocityRef, 0.25f);
            yield return 0;
        }

        counter.Add(1);

        starAdded = true;

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (!starAdded)
        {
            SaveFile.Instance().ModifyStars(1);
        }
    }
}
