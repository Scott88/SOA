using UnityEngine;
using System.Collections;

public class CashBasherPlayer : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        StartCoroutine(StartAnim());
    }

    IEnumerator StartAnim()
    {
        yield return new WaitForSeconds(Random.value);

        animator.SetInteger("costume", SaveFile.Instance().GetCurrentCostume());
        animator.SetBool("Walk", true);
    }

    public void Win()
    {
        animator.SetBool("Walk", false);
        animator.SetBool("WinAnim", false);
    }

    void PlayFootstepSound() { }
}
