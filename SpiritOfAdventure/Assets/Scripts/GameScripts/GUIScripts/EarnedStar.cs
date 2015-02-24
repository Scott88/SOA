using UnityEngine;
using System.Collections;

public class EarnedStar : MonoBehaviour
{
    public float delay { get; set; }

    private StarInventory inventory;

    private Vector3 velocityRef;

    private bool starAdded = false;

    void Start()
    {
        inventory = FindObjectOfType<StarInventory>();
    }

    public void Go()
    {
        StartCoroutine(GoToInventory());
    }

    IEnumerator GoToInventory()
    {
        yield return new WaitForSeconds(delay);

        while ((inventory.transform.position - transform.position).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, inventory.transform.position, ref velocityRef, 0.25f);
            yield return 0;
        }

        inventory.Add(1);

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
