using UnityEngine;
using System.Collections;

public class TheirTurnState : GameState 
{
    CashBasherManager manager;
    CameraMan cameraMan;

    public TheirTurnState(CashBasherManager m, CameraMan cm)
    {
        manager = m;
        cameraMan = cm;
    }

    public void Prepare()
    {
        if (Network.isServer)
        {
            cameraMan.FollowPosition(new Vector3(10f, 0f, 0f));
        }
        else
        {
            cameraMan.FollowPosition(new Vector3(-10f, 0f, 0f));
        }
    }

    public void Update()
    {

    }

    public void End()
    {

    }
}
