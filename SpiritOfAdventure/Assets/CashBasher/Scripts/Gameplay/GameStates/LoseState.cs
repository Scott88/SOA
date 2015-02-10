using UnityEngine;
using System.Collections;

public class LoseState : GameState {

	CashBasherManager manager;

    float timer = 1.5f;

    Vector3 targetPosition;

    bool treasureExplosion = true;

    public LoseState(CashBasherManager m)
    {
        manager = m;
    }

    public void Prepare()
    {
        Time.timeScale = 0.5f;

        manager.cameraMan.StopFollowing();
        manager.cameraMan.ShakeCamera(0.75f, 0.52f);
        manager.cameraMan.ZoomTo(4f);
        manager.connectionRequired = false;
    }

    public void Update()
    {
        if (treasureExplosion)
        {
            timer -= Time.deltaTime;

            if (timer < 0f)
            {
                TreasureExploded();
            }
        }
        else
        {
            if (Vector2.Distance(manager.playerCamera.transform.position, targetPosition) < 0.1f)
            {
                timer -= Time.deltaTime;
            }
        }
    }

    public void OnGUI()
    {
        if (timer < 0.0f && !treasureExplosion)
        {
            if (Network.isServer)
            {
                if (GUI.Button(new Rect(Screen.width * (0.75f), Screen.height * (0.05f), Screen.width * (0.22f), Screen.height * (0.2f)),
                               "Continue"))
                {
                    Network.Disconnect();
                    Application.LoadLevel("MiniGameMenu");
                }
            }
            else
            {
                if (GUI.Button(new Rect(Screen.width * (0.05f), Screen.height * (0.05f), Screen.width * (0.22f), Screen.height * (0.2f)),
                               "Continue"))
                {
                    Network.Disconnect();
                    Application.LoadLevel("MiniGameMenu");
                }
            }
        }
    }

    public void End()
    {

    }

    public void TreasureExploded()
    {
        treasureExplosion = false;
        timer = 2.0f;

        Time.timeScale = 1.0f;

        if (Network.isServer)
        {
            targetPosition = new Vector3(-4.5f, 1f, 0f);
        }
        else
        {
            targetPosition = new Vector3(4.5f, 1f, 0f);
        }

        manager.cameraMan.ZoomTo(7f);
        manager.cameraMan.FollowPosition(targetPosition);
    }
}
