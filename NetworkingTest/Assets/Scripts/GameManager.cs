using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public TileManager tileManager;

    public Camera playerCamera;
    public TextMesh myDisplay, theirDisplay, turnDisplay;

    public BlockInventory playerInventory;

    public GameObject spawnedBlock;
    public GameObject spawnIndicator;

    public TextMesh winText;

    public int myTeam { get; set; }

    private bool myTurn = false;

    private ArrayList breakables;

    private int myPoints, theirPoints;

    private float leaveTimer = 3.0f;
    private bool gameOver = false;

	void Awake()
	{
		breakables = new ArrayList();
	}

    void Start()
    {
        myTeam = -1;
    }

    public void InitGame()
    {
        networkView.RPC("InitializeGame", RPCMode.All);
    }

    [RPC]
    void InitializeGame()
    {
        if (PlayerPrefs.HasKey("Points"))
        {
            myPoints = PlayerPrefs.GetInt("Points");
        }
        else
        {
            myPoints = 15;
            PlayerPrefs.SetInt("Points", myPoints);
        }

        myDisplay.text = "Your score: " + myPoints.ToString();
        networkView.RPC("SetEnemyPoints", RPCMode.OthersBuffered, myPoints);

        turnDisplay.text = "Their turn...";
    }

    public void AddBreakable(Breakable b)
    {
        breakables.Add(b);
    }

    void Update()
    {
        if (!gameOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GetClickedOn();
            }
            else if (Input.GetMouseButton(0))
            {
                GetHeldOn();
            }

            if (Input.GetMouseButtonUp(0))
            {
                GetReleasedOn();
            }
        }
        else
        {
            leaveTimer -= Time.deltaTime;

            if (leaveTimer <= 0.0f)
            {
                Network.Disconnect();
            }
        }
    }

    void GetClickedOn()
    {
        Ray clickRay = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(clickRay, out hit))
        {
            BlockInventory inventory = hit.collider.gameObject.GetComponent<BlockInventory>();

            if (inventory)
            {
                if (playerInventory.IsSelected())
                {
                    DeselectInventory();
                }
                else
                {
                    SelectInventory();
                }

                return;
            }
        }
    }

    void GetHeldOn()
    {
        if (playerInventory.IsSelected())
        {
            if (!spawnIndicator.activeSelf)
            {
                spawnIndicator.SetActive(true);
            }

            Vector3 position = playerCamera.ScreenToWorldPoint(Input.mousePosition);

            position.x = Mathf.Floor(position.x) + 0.5f;
            position.y = Mathf.Floor(position.y) + 0.5f;
            position.z = 0;

            spawnIndicator.transform.position = position;
        }
    }

    void GetReleasedOn()
    {
        Ray clickRay = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (playerInventory.IsSelected())
        {
            spawnIndicator.SetActive(false);
        }

        if (Physics.Raycast(clickRay, out hit))
        {
            Breakable breakable = hit.collider.gameObject.GetComponent<Breakable>();

            if (breakable)
            {
                if (!playerInventory.IsSelected() && myTurn)
                {
                    TryBreakBlock(breakable);
                }

                return;
            }

            WinButton win = hit.collider.gameObject.GetComponent<WinButton>();

            if (win)
            {
                if (!playerInventory.IsSelected() && myTurn)
                {
                    networkView.RPC("WinScreen", RPCMode.All, win.GetTeam());
                }

                return;
            }
        }

        if (playerInventory.IsSelected())
        {
            PlaceBlock(playerCamera.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    void TryBreakBlock(Breakable breakable)
    {
        if (breakable.GetTeam() != myTeam)
        {
            if (breakable.Break())
            {
                myPoints += 5;
                myDisplay.text = "Your Points: " + myPoints.ToString();

                theirPoints -= 5;
                theirDisplay.text = "Their Points: " + theirPoints.ToString();

                tileManager.FreeTile(breakable.transform.position);
                networkView.RPC("BreakBlock", RPCMode.Others, breakable.networkView.viewID);

                breakables.Remove(breakable);
            }
            else
            {
                networkView.RPC("MyTurn", RPCMode.Others);
            }

            turnDisplay.text = "Their turn...";
            myTurn = false;
        }

    }

    [RPC]
    void BreakBlock(NetworkViewID id)
    {
        bool found = false;

        for (int j = 0; j < breakables.Count && !found; j++)
        {
            Breakable b = breakables[j] as Breakable;

            if (b.networkView.viewID == id)
            {
                found = true;

                b.Break();
                breakables.Remove(b);

                myPoints -= 5;
                myDisplay.text = "Your score: " + myPoints.ToString();

                theirPoints += 5;
                theirDisplay.text = "Their score: " + theirPoints.ToString();
            }
        }

        turnDisplay.text = "Your turn...";
        myTurn = true;
    }

    void SelectInventory()
    {
        playerInventory.Select();
    }

    void PlaceBlock(Vector3 position)
    {
        position.x = Mathf.Floor(position.x) + 0.5f;
        position.y = Mathf.Floor(position.y) + 0.5f;
        position.z = 0;

        GameObject block = Network.Instantiate(spawnedBlock, position, new Quaternion(), 0) as GameObject;

        Breakable b = block.GetComponent<Breakable>();
        b.SetTeam(myTeam);

        playerInventory.Deselect(true);
    }

    void DeselectInventory()
    {
        playerInventory.Deselect(false);
    }

    [RPC]
    void WinScreen(int winningTeam)
    {
        if (winningTeam != myTeam)
        {
            winText.text = "YOU WIN!";
        }
        else
        {
            winText.text = "You lose...";
        }

        gameOver = true;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width * (0.8f), Screen.height * (0.8f), Screen.width * (0.18f), Screen.height * (0.18f)),
                           "Disconnect"))
        {
            if (myPoints < 0)
            {
                PlayerPrefs.SetInt("Points", 0);
            }
            else
            {
                PlayerPrefs.SetInt("Points", myPoints);
            }

            Network.Disconnect();
            Application.LoadLevel("menu");
        }
    }

    [RPC]
    public void MyTurn()
    {
        turnDisplay.text = "Your turn...";
        myTurn = true;
    }

    [RPC]
    void SetEnemyPoints(int points)
    {
        theirPoints = points;
        theirDisplay.text = "Their score: " + theirPoints.ToString();
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        if (myPoints < 0)
        {
            PlayerPrefs.SetInt("Points", 0);
        }
        else
        {
            PlayerPrefs.SetInt("Points", myPoints);
        }

        playerInventory.Deselect(false);
        playerInventory.Save();

        Application.LoadLevel("menu");
    }

	void OnPlayerDisconnected()
    {
        if (myPoints < 0)
        {
            PlayerPrefs.SetInt("Points", 0);
        }
        else
        {
            PlayerPrefs.SetInt("Points", myPoints);
        }

        playerInventory.Deselect(false);
        playerInventory.Save();

        Network.Disconnect();
        Application.LoadLevel("menu");
    }

    
}
