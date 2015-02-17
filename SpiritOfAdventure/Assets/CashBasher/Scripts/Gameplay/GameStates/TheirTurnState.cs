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
                manager.cameraMan.FollowWaypoint(manager.clientTileFocus);  
            }
            else
            {
                manager.cameraMan.FollowWaypoint(manager.serverTileFocus);
            }

            manager.cameraMan.ZoomTo(5f);

            yield return new WaitForSeconds(3.0f);
        }

        if (Network.isServer)
        {
            manager.cameraMan.FollowWaypoint(manager.serverCamFocus);
        }
        else
        {
            manager.cameraMan.FollowWaypoint(manager.clientCamFocus);
        }
    }
}
