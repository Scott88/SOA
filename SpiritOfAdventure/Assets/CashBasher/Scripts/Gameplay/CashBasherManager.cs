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
    public float startTimer = 2.0f;
    public float buildTimer = 30.0f;

    public Camera playerCamera, guiCamera;

    public CameraMan cameraMan;

    public NetworkedTileSet serverSet, clientSet;

    public NetworkedCannon serverCannon, clientCannon;

    public CashBasherSpirit greenSpirit, blueSpirit, redSpirit;

    public GameObject serverSpiritWaypoint, clientSpiritWaypoint;

    public TextMesh gameText;

    public GameObject treasure;
    public GameObject treasureSupport;

    public GameObject spiritButtons;

    public GUISkin mySkin;

    public int myTeam { get; set; }

    public bool opponentIsReady { get; set; }

    public bool connectionRequired { get; set; }

    private GameState state = null;
    private GamePhase currentPhase = GamePhase.GP_STARTING;

    private NetworkedLevelLoader loader;

    private StartState startState;
    //private BuildState buildState;
    //private WaitingState waitingState;
    private YourTurnState yourTurnState;
    private TheirTurnState theirTurnState;
    private WinState winState;
    private LoseState loseState;

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
    }

    void Start()
    {
        loader = FindObjectOfType<NetworkedLevelLoader>();

        if (Network.isServer)
        {
            myTeam = 0;
            cameraMan.FollowPosition(new Vector3(-10f, 0f, 0f));
        }
        else if (Network.isClient)
        {
            myTeam = 1;
            cameraMan.FollowPosition(new Vector3(10f, 0f, 0f)); 
        }
        
        //cameraMan.ZoomTo(4f);

        startState = new StartState(this, startTimer, loader);
        //buildState = new BuildState(this, myTeam, myTeam == 0 ? serverSet : clientSet, buildTimer);
        //waitingState = new WaitingState(this);
        yourTurnState = new YourTurnState(this, myTeam == 0 ? serverCannon : clientCannon, cameraMan, myTeam == 0 ? serverSpiritWaypoint : clientSpiritWaypoint, spiritButtons);
        theirTurnState = new TheirTurnState(this, cameraMan);
        winState = new WinState(this);
        loseState = new LoseState(this);

        SwitchToState((int)GamePhase.GP_STARTING);
    }

    void Update()
    {
        if (state != null)
        {
            state.Update();
        }
    }

    void OnGUI()
    {
        if (state != null)
        {
            GUI.skin = mySkin;

            state.OnGUI();
        }
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

    void OnApplicationPause()
    {
        SaveFile.Instance().SaveToXML();
    }
#endif

}
