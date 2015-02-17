using UnityEngine;
using System.Collections;

enum GamePhase
{
    GP_STARTING = 0,
    //GP_BUILD = 1,
    //GP_WAITING = 2,
    GP_YOUR_TURN = 3,
    GP_THEIR_TURN = 4,
    GP_WIN = 5,
    GP_LOSE = 6
}

public class CashBasherManager : MonoBehaviour
{
    public float blockSpiritChance = 0.1f;
    public int minimumSpirits = 1;

    public Camera playerCamera, guiCamera;

    public CameraMan cameraMan;

    public NetworkedTileSet serverSet, clientSet;

    public NetworkedCannon serverCannon, clientCannon;

    public CashBasherSpiritGUI greenGUI, blueGUI, redGUI;

    public GameObject greenTransfer, blueTransfer, redTransfer;

    public CameraWaypoint serverCamFocus, clientCamFocus;

    public CameraWaypoint serverTileFocus, clientTileFocus;

    public GameObject serverWaypoint, clientWaypoint;

    public GameObject trappedGreenSpirit, trappedBlueSpirit, trappedRedSpirit;

    public TextMesh gameText;

    public GameObject treasure;
    public GameObject treasureSupport;

    public GameObject splashScreen;

    public GUISkin mySkin;

    public StartState startState;
    public YourTurnState yourTurnState;
    public TheirTurnState theirTurnState;
    public WinState winState;
    public LoseState loseState;

    public GameObject pauseScreen;

    public bool paused { get; set; } 

    public int myTeam { get; set; }

    public bool opponentIsReady { get; set; }

    public bool connectionRequired { get; set; }

    private bool serverHasEffects = false, clientHasEffects = false;

    private GameState state = null;
    private GamePhase currentPhase = GamePhase.GP_STARTING;

    private NetworkedLevelLoader loader;

    void Awake()
    {
        if (Network.isServer)
        {
            myTeam = 0;
        }
        else if (Network.isClient)
        {
            myTeam = 1;
        }

        connectionRequired = true;

        splashScreen.SetActive(true);
    }

    void Start()
    {
        loader = FindObjectOfType<NetworkedLevelLoader>();

        if (Network.isServer)
        {
            myTeam = 0;
            cameraMan.FollowPosition(new Vector3(-4.5f, 1f, 0f));
        }
        else if (Network.isClient)
        {
            myTeam = 1;
            cameraMan.FollowPosition(new Vector3(4.5f, 1f, 0f)); 
        }

        SwitchToState((int)GamePhase.GP_STARTING);
    }

    void Update()
    {
        if (state != null)
        {
            state.UpdateState();
        }
    }

    void OnGUI()
    {
        if (state != null)
        {
            GUI.skin = mySkin;

            state.OnStateGUI();
        }
    }

    public void GenerateServerSpirits()
    {
        serverSet.GenerateBlockSpirits(blockSpiritChance, minimumSpirits);
    }

    public void GenerateClientSpirits()
    {
        networkView.RPC("NetGenerateClientSpirits", RPCMode.Others);
    }

    [RPC]
    void NetGenerateClientSpirits()
    {
        clientSet.GenerateBlockSpirits(blockSpiritChance, minimumSpirits);
    }

    public NetworkedCannon GetCannon(bool serversCannon)
    {
        if (serversCannon)
        {
            return serverCannon;
        }
        else
        {
            return clientCannon;
        }
    }

    public NetworkedTileSet GetTileSet(bool server)
    {
        if (server)
        {
            return serverSet;
        }
        else
        {
            return clientSet;
        }
    }

    public void CreateBlockSpirit(Breakable block)
    {
        GameObject blockSpirit = null;

        switch (block.containedSpirit)
        {
            case SpiritType.ST_GREEN:
                blockSpirit = Instantiate(trappedGreenSpirit, block.transform.position + Vector3.back * 2, Quaternion.identity) as GameObject;
                break;
            case SpiritType.ST_BLUE:
                blockSpirit = Instantiate(trappedBlueSpirit, block.transform.position + Vector3.back * 2, Quaternion.identity) as GameObject;
                break;
            case SpiritType.ST_RED:
                blockSpirit = Instantiate(trappedRedSpirit, block.transform.position + Vector3.back * 2, Quaternion.identity) as GameObject;
                break;
        }

        blockSpirit.transform.parent = block.transform;
    }

    [RPC]
    void FadeSplashScreen()
    {
        StartCoroutine(FadeSplash());
    }

    IEnumerator FadeSplash()
    {
        Color color = splashScreen.renderer.material.color;

        while (color.a > 0f)
        {
            color.a -= Time.deltaTime;
            splashScreen.renderer.material.color = color;

            yield return null;
        }

        splashScreen.SetActive(false);
    }

    [RPC]
    void UpdateEffectStatus()
    {
        if (Network.isServer)
        {
            networkView.RPC("NetSetEffectStatus", RPCMode.All, true, serverSet.HasStatusEffects());
        }
        else
        {
            networkView.RPC("NetSetEffectStatus", RPCMode.All, false, clientSet.HasStatusEffects());
        }
    }

    public void SetEffectStatus(NetworkedTileSet set, bool effects)
    {
        if (set == serverSet)
        {
            networkView.RPC("NetSetEffectStatus", RPCMode.All, true, effects);
        }
        else if (set == clientSet)
        {
            networkView.RPC("NetSetEffectStatus", RPCMode.All, false, effects);
        }
    }

    [RPC]
    void NetSetEffectStatus(bool server, bool effects)
    {
        if (server)
        {
            serverHasEffects = effects;
        }
        else
        {
            clientHasEffects = effects;
        }
    }

    public bool HasEffects(bool server)
    {
        if (server)
        {
            return serverHasEffects;
        }
        else
        {
            return clientHasEffects;
        }
    }

    public void TransferSpirit(SpiritType type, Vector3 position, bool myBlock)
    {
        switch (type)
        {
            case SpiritType.ST_GREEN:
                TransferSpirit(greenGUI, greenTransfer, position, myBlock);
                break;
            case SpiritType.ST_BLUE:
                TransferSpirit(blueGUI, blueTransfer, position, myBlock);
                break;
            case SpiritType.ST_RED:
                TransferSpirit(redGUI, redTransfer, position, myBlock);
                break;
        }     
    }

    void TransferSpirit(CashBasherSpiritGUI gui, GameObject transferPref, Vector3 position, bool myBlock)
    {
        GameObject transferedSpirit = Instantiate(transferPref, position, Quaternion.identity) as GameObject;

        SmoothDamper damper = transferedSpirit.GetComponent<SmoothDamper>();

        if (myBlock)
        {
            if (Network.isServer)
            {
                damper.target = clientWaypoint.transform.position;
            }
            else
            {
                damper.target = serverWaypoint.transform.position;
            }
        }
        else
        {
            gui.Add();

            if (Network.isServer)
            {
                damper.target = serverWaypoint.transform.position;
            }
            else
            {
                damper.target = clientWaypoint.transform.position;
            }
        }
    }

    public void RandomizeTreasure()
    {
        int x = Random.Range(0, 3);
        int y = Random.Range(0, 4);

        networkView.RPC("PlaceTreasureAndLoad", RPCMode.All, x, y);
    }

    public void ReadyNextTurn()
    {
        yourTurnState.ReadyNextTurn();
    }

    [RPC]
    void PlaceTreasureAndLoad(int x, int y)
    {
        if (Network.isClient)
        {
            clientSet.LoadNetworkedBlock(x, y, treasure);

            for (int j = y - 1; j >= 0; j--)
            {
                clientSet.LoadNetworkedBlock(x, j, treasureSupport);
            }

            clientSet.Load();
        }
        else
        {
            serverSet.LoadNetworkedBlock(x, y, treasure);

            for (int j = y - 1; j >= 0; j--)
            {
                serverSet.LoadNetworkedBlock(x, j, treasureSupport);
            }

            serverSet.Load();
        }
    }

    [RPC]
    void FocusCamera(bool left)
    {
        if (left)
        {
            cameraMan.FollowWaypoint(serverCamFocus);
        }
        else if (!left)
        {
            cameraMan.FollowWaypoint(clientCamFocus);
        }
    }

    [RPC]
    public void SwitchToState(int phase)
    {
        currentPhase = (GamePhase)(phase);

        if (state != null)
        {
            state.End();
        }

        switch (currentPhase)
        {
            case GamePhase.GP_STARTING:
                state = startState;              
                break;
            //case GamePhase.GP_BUILD:
                //state = buildState;              
                //break;
            //case GamePhase.GP_WAITING:
                //state = waitingState;
                //gameText.text = "Waiting for your friend...";
                //break;
            case GamePhase.GP_YOUR_TURN:
                state = yourTurnState;
                gameText.text = "Your turn!";
                break;
            case GamePhase.GP_THEIR_TURN:
                state = theirTurnState;
                gameText.text = "Their turn...";
                break;
            case GamePhase.GP_WIN:
                state = winState;
                gameText.text = "YOU WIN!";
                break;
            case GamePhase.GP_LOSE:
                state = loseState;
                gameText.text = "You lose...";
                break;
        }

        if (state != null)
        {
            state.Prepare();
        }
    }

    [RPC]
    void OpponentReady()
    {
        opponentIsReady = true;
    }

    void OnApplicationPause(bool pauseState)
    {
        if (connectionRequired)
        {
            networkView.RPC("NetworkedPause", RPCMode.Others, pauseState);
        }

#if UNITY_EDITOR
        if (pauseState)
        {
            SaveFile.Instance().SaveToXML();
        }
#endif
    }

    [RPC]
    void NetworkedPause(bool pauseState)
    {
        Time.timeScale = pauseState ? 0f : 1f;
        paused = pauseState;
        pauseScreen.SetActive(pauseState);
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        if (connectionRequired)
        {
            Application.LoadLevel("MiniGameMenu");
        }
    }

    void OnPlayerDisconnected()
    {
        Network.Disconnect();

        if (connectionRequired)
        {         
            Application.LoadLevel("MiniGameMenu");
        }
    }

    

#if UNITY_EDITOR
    void OnApplicationQuit()
    {
        SaveFile.Instance().SaveToXML();
    } 
#endif

}
