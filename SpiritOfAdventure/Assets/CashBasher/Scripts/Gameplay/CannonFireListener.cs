using UnityEngine;
using System.Collections;

public class CannonFireListener : MonoBehaviour {

    public Cannon cannon;
    public Animator smokeAnimator;
    public Animator groundSmokeAnimator;

    void FireCannon(int largeSmoke)
    {
        cannon.Fire();

        smokeAnimator.SetBool("BigSmoke", largeSmoke == 0 ? false : true);
        smokeAnimator.SetTrigger("Fire");

        if (largeSmoke == 2 && transform.parent.transform.eulerAngles.z < 30f)
        {
            groundSmokeAnimator.SetTrigger("Fire");
        }
    }
}
 