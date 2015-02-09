using UnityEngine;
using System.Collections;

public class LoseState : GameState {

	CashBasherManager manager;

    float timer = 3.0f;

    Vector3 targetPosition;

    public LoseState(CashBasherManager m, Vector3 pos)
    {
        manager = m;
        targetPosition = pos;
    }

    public void Prepare()
    {
        manager.cameraMan.FollowPosition(targetPosition);
        manager.connectionRequired = false;
    }

    public void Update()
    {
        if (Vector3.Distance(manager.playerCamera.transform.position, targetPosition) < 0.1f)
        {
            timer -= Time.deltaTime;

            if (timer < 0.0f)
            {
                Application.LoadLevel("MiniGameMenu");
            }
        }
    }

    public void End()
    {

    }
}
