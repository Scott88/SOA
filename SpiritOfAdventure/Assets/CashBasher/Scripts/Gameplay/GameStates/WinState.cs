using UnityEngine;
using System.Collections;

public class WinState : GameState
{
	public CashBasherManager manager;

    public GameObject spiritButtons;

    public CashBasherPlayer serverPlayer, clientPlayer;

    public GameObject endGameDialogue, winScreen;

    public float targetX = -70f;

    private bool treasureExplosion = true;

    private Vector3 buttonsPos;
    private Vector3 velocityRef;

    private CashBasherPlayer myPlayer;

    private Vector3 targetPos, dialogueRef;

    void Start()
    {
        buttonsPos = spiritButtons.transform.position;

        myPlayer = Network.isServer ? serverPlayer : clientPlayer;

        targetPos = new Vector3(targetX, endGameDialogue.transform.position.y);
    }

    public override void Prepare()
    {
        //Time.timeScale = 0.5f;

        //manager.cameraMan.StopFollowing();
        //manager.cameraMan.ShakeCamera(0.75f, 0.52f);
        //manager.cameraMan.ZoomTo(4f);
        //manager.connectionRequired = false;

        //winScreen.SetActive(true);

        //myPlayer.Win();

        StartCoroutine(Preshow());
    }

    IEnumerator Preshow()
    {
        Time.timeScale = 0.5f;

        manager.cameraMan.StopFollowing();
        manager.cameraMan.ShakeCamera(0.75f, 0.52f);
        manager.cameraMan.ZoomTo(4f);
        manager.connectionRequired = false;

        winScreen.SetActive(true);

        myPlayer.Win();

        yield return new WaitForSeconds(0.5f);

        Network.Disconnect();

        yield return new WaitForSeconds(1.0f);

        treasureExplosion = false;

        Time.timeScale = 1.0f;

        while (FindObjectOfType<StarSpawner>())
        {
            yield return 0;
        }

        yield return new WaitForSeconds(1.2f);

        FindObjectOfType<StarCounter>().TransferStars();

        if (Network.isServer)
        {
            manager.cameraMan.FollowWaypoint(manager.serverCamFocus);
        }
        else
        {
            manager.cameraMan.FollowWaypoint(manager.clientCamFocus);
        }
    }

    public override void UpdateState()
    {
        spiritButtons.transform.position = Vector3.SmoothDamp(spiritButtons.transform.position, buttonsPos, ref velocityRef, 1f);

        if (!treasureExplosion)
        {
            endGameDialogue.transform.position = Vector3.SmoothDamp(endGameDialogue.transform.position, targetPos, ref dialogueRef, 0.3f);
        }
    }

    public override void OnStateGUI()
    {
        if (!treasureExplosion)
        {
            if (GUI.Button(new Rect(Screen.width * (0.75f), Screen.height * (0.05f), Screen.width * (0.22f), Screen.height * (0.2f)),
                           "Continue"))
            {
                LevelQueue.LoadLevel("MiniGameMenu");
            }
        }
    }

    public void TreasureExploded()
    {
        treasureExplosion = false;

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
