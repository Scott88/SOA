using UnityEngine;
using System.Collections;

public class CashBasherPlayer : MonoBehaviour
{
    public int team;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        StartCoroutine(StartAnim());
    }

    IEnumerator StartAnim()
    {
        yield return new WaitForSeconds(Random.value);

        if (team == 0 && Network.isServer || team == 1 && Network.isClient)
        {
            //animator.SetInteger("costume", SaveFile.Instance().GetCurrentCostume());
            animator.SetInteger("costume", FindObjectOfType<NetLevelHandshake>().myCostume);
        }
        else
        {
            animator.SetInteger("costume", FindObjectOfType<NetworkedLevelLoader>().othersCostume);
        }

        animator.SetBool("Walk", true);
    }

    public void Win()
    {
        animator.SetBool("Walk", false);
        animator.SetBool("WinAnim", false);
    }

    void PlayFootstepSound() { }
}
