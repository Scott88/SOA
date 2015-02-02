﻿using UnityEngine;
using System.Collections;

public class YourTurnState : GameState
{
    CashBasherManager manager;
    NetworkedCannon yourCannon;
    CameraMan cameraMan;

    GameObject cannonBall = null;
    bool ballRecieved = false;

    float timer = 1.0f;

    public YourTurnState(CashBasherManager m, NetworkedCannon c, CameraMan cm)
    {
        manager = m;
        yourCannon = c;
        cameraMan = cm;
    }

    public void Prepare()
    {
        if (Network.isServer)
        {
            cameraMan.FollowPosition(new Vector3(-10f, 0f, 0f));
        }
        else
        {
            cameraMan.FollowPosition(new Vector3(10f, 0f, 0f));
        }

        yourCannon.Activate();
    }

    public void SetCannonBall(GameObject ball)
    {
        cannonBall = ball;
        ballRecieved = true;
    }

    public void Update()
    {
        if (ballRecieved)
        {
            if (cannonBall == null)
            {
                timer -= Time.deltaTime;

                if (timer <= 0.0f)
                {
                    manager.networkView.RPC("SwitchToState", RPCMode.Others, (int)GamePhase.GP_YOUR_TURN);
                    manager.SwitchToState((int)GamePhase.GP_THEIR_TURN);
                }
            }
        }
    }

    public void End()
    {
        yourCannon.Deactivate();
        ballRecieved = false;
    }
}