using UnityEngine;
using System.Collections;

public class TheirTurnState : GameState 
{
    public CashBasherManager manager;

    public override void Prepare()
    {
        manager.StartCoroutine(Preshow());
    }

    IEnumerator Preshow()
    {
        if (manager.HasEffects(Network.isClient))
        {
            if (Network.isServer)
            {
                manager.cameraMan.FollowPosition(new Vector3(10f, 0f, 0f));
            }
            else
            {
                manager.cameraMan.FollowPosition(new Vector3(-10f, 0f, 0f));
            }

            manager.cameraMan.ZoomTo(5f);

            yield return new WaitForSeconds(3.0f);
        }

        if (Network.isServer)
        {
            manager.cameraMan.FollowPosition(new Vector3(-4.5f, 1f, 0f));
        }
        else
        {
            manager.cameraMan.FollowPosition(new Vector3(4.5f, 1f, 0f));
        }

        manager.cameraMan.ZoomTo(7f);
    }
}
