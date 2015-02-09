﻿using UnityEngine;
using System.Collections;

public class LoseState : GameState {

	CashBasherManager manager;

    float timer = 3.0f;

    Vector3 targetPosition;

    public LoseState(CashBasherManager m)
    {
        manager = m;
    }

    public void Prepare()
    {
        if (Network.isServer)
        {
            targetPosition = new Vector3(4.5f, 1f, 0f);
        }
        else
        {
            targetPosition = new Vector3(-4.5f, 1f, 0f);
        }

        manager.cameraMan.FollowPosition(targetPosition);
        manager.connectionRequired = false;
    }

    public void Update()
    {
        if (Vector2.Distance(manager.playerCamera.transform.position, targetPosition) < 0.1f)
        {
            timer -= Time.deltaTime;
        }
    }

    public void OnGUI()
    {
        if (timer < 0.0f)
        {
            if (Network.isServer)
            {
                if (GUI.Button(new Rect(Screen.width * (0.75f), Screen.height * (0.05f), Screen.width * (0.22f), Screen.height * (0.2f)),
                               "Continue"))
                {
                    Application.LoadLevel("MiniGameMenu");
                }
            }
            else
            {
                if (GUI.Button(new Rect(Screen.width * (0.05f), Screen.height * (0.05f), Screen.width * (0.22f), Screen.height * (0.2f)),
                               "Continue"))
                {
                    Application.LoadLevel("MiniGameMenu");
                }
            }
        }
    }

    public void End()
    {

    }
}
