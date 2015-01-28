using UnityEngine;
using System.Collections;

public class CashBasherManager : MonoBehaviour
{
    public float startTimer = 2.0f;
    public float buildTimer = 30.0f;

    public Camera playerCamera, guiCamera;

    public CameraMan cameraMan;

    public TileSet serverSet, clientSet;

    public TextMesh gameText;

    public GameObject treasure;
    public GameObject treasureSupport;

    public GameObject spawnIndicator;

    public int myTeam;

    private GameState state = null;
    private GamePhase currentPhase = GamePhase.GP_STARTING;

    private bool opponentIsReady = false;

    private ArrayList blocks;

    private NetworkedLevelLoader loader;

    private BuildState buildState;

    enum GamePhase
    {
        GP_STARTING = 0,
        GP_BUILD = 1,
        GP_WAITING = 2,
        GP_YOUR_TURN = 3,
        GP_THEIR_TURN = 4,
        GP_OVER = 5
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

        buildState = new BuildState(this, myTeam, myTeam == 0 ? serverSet : clientSet, spawnIndicator);
    }

    public void AddBlock(Breakable b)
    {
        blocks.Add(b);
    }

    void Update()
    {
        if (currentPhase == GamePhase.GP_STARTING)
        {
            startTimer -= Time.deltaTime;

            if (startTimer <= 0f)
            {
                if (Network.isServer && loader.IsReady())
                {
                    RandomizeTreasure();

                    networkView.RPC("SwitchToState", RPCMode.All, (int)GamePhase.GP_BUILD);
                }
            }
        }

        if (currentPhase == GamePhase.GP_BUILD)
        {
            if (buildTimer > 0f)
            {
                buildTimer -= Time.deltaTime;
            }
            else
            {
                if (Network.isClient)
                {
                    networkView.RPC("OpponentReady", RPCMode.Server);
                }

                SwitchToState((int)GamePhase.GP_WAITING);
            }
        }

        if (currentPhase == GamePhase.GP_WAITING)
        {
            if (Network.isServer && opponentIsReady)
            {

            }
        }

        if (state != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                state.GetClickedOn();
            }
            else if (Input.GetMouseButton(0))
            {
                state.GetHeldOn();
            }

            if (Input.GetMouseButtonUp(0))
            {
                state.GetReleasedOn();
            }
        }
    }

    void RandomizeTreasure()
    {
        int x = Random.Range(0, 9);
        int y = Random.Range(0, 7);

        networkView.RPC("PlaceTreasure", RPCMode.All, x, y);
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

        for (int j = y - 1; y >= 0; y--)
        {
            treasurePosition.y -= 1f;
            Network.Instantiate(treasureSupport, treasurePosition, new Quaternion(), 0);
        }
    }

    [RPC]
    void SwitchToState(int phase)
    {
        currentPhase = (GamePhase)(phase);

        if (state != null)
        {
            state.End();
        }

        switch (currentPhase)
        {
            case GamePhase.GP_STARTING:
                state = null;
                break;
            case GamePhase.GP_BUILD:
                state = buildState;
                break;
            case GamePhase.GP_WAITING:
                state = null;
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
}
