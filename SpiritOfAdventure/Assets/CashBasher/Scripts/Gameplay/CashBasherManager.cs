using UnityEngine;
using System.Collections;

enum GamePhase
{
    GP_STARTING = 0,
    GP_BUILD = 1,
    GP_WAITING = 2,
    GP_YOUR_TURN = 3,
    GP_THEIR_TURN = 4,
    GP_OVER = 5
}

public class CashBasherManager : MonoBehaviour
{
    public float startTimer = 2.0f;
    public float buildTimer = 30.0f;

    public Camera playerCamera, guiCamera;

    public CameraMan cameraMan;

    public TileSet serverSet, clientSet;

    public NetworkedCannon serverCannon, clientCannon;

    public TextMesh gameText;

    public GameObject treasure;
    public GameObject treasureSupport;

    public GameObject spawnIndicator;

    public int myTeam;

    public bool opponentIsReady { get; set; }

    private GameState state = null;
    private GamePhase currentPhase = GamePhase.GP_STARTING;

    private ArrayList blocks;

    private NetworkedLevelLoader loader;

    private StartState startState;
    private BuildState buildState;
    private WaitingState waitingState;
    private YourTurnState yourTurnState;
    private TheirTurnState theirTurnState;

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
    }

    void Start()
    {
        loader = FindObjectOfType<NetworkedLevelLoader>();

        blocks = new ArrayList();

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
        buildState = new BuildState(this, myTeam, myTeam == 0 ? serverSet : clientSet, spawnIndicator, buildTimer);
        waitingState = new WaitingState(this);
        yourTurnState = new YourTurnState(this, myTeam == 0 ? serverCannon : clientCannon, cameraMan);
        theirTurnState = new TheirTurnState(this, cameraMan);

		state = startState;
    }

    public void AddBlock(Breakable b)
    {
        blocks.Add(b);
    }

    void Update()
    {
        if (state != null)
        {
            state.Update();
        }
    }

    public void RandomizeTreasure()
    {
        int x = Random.Range(0, 9);
        int y = Random.Range(0, 7);

        networkView.RPC("PlaceTreasure", RPCMode.All, x, y);
    }

    public void ReadyNextTurn()
    {
        yourTurnState.ReadyNextTurn();
    }

    [RPC]
    void PlaceTreasure(int x, int y)
    {
        Vector3 treasurePosition;

        if (Network.isClient)
        {
            x = 7 - x;
            treasurePosition = clientSet.GetPositionFromCoords(x, y);
        }
        else
        {
            treasurePosition = serverSet.GetPositionFromCoords(x, y);
        }

        Network.Instantiate(treasure, treasurePosition, new Quaternion(), 0);

        for (int j = y - 1; j >= 0; j--)
        {
            treasurePosition.y -= 1f;
            Network.Instantiate(treasureSupport, treasurePosition, new Quaternion(), 0);
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
            case GamePhase.GP_BUILD:
                state = buildState;
                break;
            case GamePhase.GP_WAITING:
                state = waitingState;
                break;
            case GamePhase.GP_YOUR_TURN:
                state = yourTurnState;
                break;
            case GamePhase.GP_THEIR_TURN:
                state = theirTurnState;
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
        Application.LoadLevel("MiniGameMenu");
    }

    void OnPlayerDisconnected()
    {
        Network.Disconnect();
        Application.LoadLevel("MiniGameMenu");
    }
}
