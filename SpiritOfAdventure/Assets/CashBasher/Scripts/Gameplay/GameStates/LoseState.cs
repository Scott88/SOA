using UnityEngine;
using System.Collections;

public class LoseState : GameState {

	public CashBasherManager manager;

    public GameObject spiritButtons;

    public GameObject endGameDialogue, loseScreen;

    public float targetX = -70f;

    private float timer = 1.5f;

    private bool treasureExplosion = true;

    private Vector3 buttonsPos;
    private Vector3 velocityRef;

    private Vector3 targetPos, dialogueRef;

    void Start()
    {
        buttonsPos = spiritButtons.transform.position;

        targetPos = new Vector3(targetX, endGameDialogue.transform.position.y);
    }

    public override void Prepare()
    {
        Time.timeScale = 0.5f;

        manager.cameraMan.StopFollowing();
        manager.cameraMan.ShakeCamera(0.75f, 0.52f);
        manager.cameraMan.ZoomTo(4f);
        manager.connectionRequired = false;

        loseScreen.SetActive(true);
    }

    public override void UpdateState()
    {
        spiritButtons.transform.position = Vector3.SmoothDamp(spiritButtons.transform.position, buttonsPos, ref velocityRef, 1f);

        timer -= Time.deltaTime;

        if (treasureExplosion)
        {
            if (timer < 0f)
            {
                TreasureExploded();
            }
        }
        else
        {
            endGameDialogue.transform.position = Vector3.SmoothDamp(endGameDialogue.transform.position, targetPos, ref dialogueRef, 0.3f);
        }
    }

    public override void OnStateGUI()
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

    public void TreasureExploded()
    {
        treasureExplosion = false;
        timer = 2.0f;

        Time.timeScale = 1.0f;

        FindObjectOfType<StarCounter>().TransferStars(0.5f);

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
