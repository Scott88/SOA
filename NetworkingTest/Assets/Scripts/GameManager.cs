using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public Camera playerCamera;
    public TextMesh myDisplay, theirDisplay, turnDisplay;

    public BlockInventory playerInventory;

    public GameObject spawnedBlock;

    public int myTeam { get; set; }

    private bool myTurn = false;

    private ArrayList breakables;

    private int myPoints, theirPoints;

	void Awake()
	{
		breakables = new ArrayList();
	}

    void Start()
    {
        myTeam = -1;
    }

    public void AddBreakable(Breakable b)
    {
        breakables.Add(b);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetClickedOn();
        }
    }

    void GetClickedOn()
    {
        Ray clickRay = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

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

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width * (0.8f), Screen.height * (0.8f), Screen.width * (0.18f), Screen.height * (0.18f)),
                           "Disconnect"))
        {
            if (myPoints < 15)
            {
                PlayerPrefs.SetInt("Points", 15);
            }
            else
            {
                PlayerPrefs.SetInt("Points", myPoints);
            }

            FindObjectOfType<Networker>().Reset();

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
        if (myPoints < 15)
        {
            PlayerPrefs.SetInt("Points", 15);
        }
        else
        {
            PlayerPrefs.SetInt("Points", myPoints);
        }

        playerInventory.Deselect(false);
        playerInventory.Save();

        FindObjectOfType<Networker>().Reset();

        Application.LoadLevel("menu");
    }

	void OnPlayerDisconnected()
    {
        if (myPoints < 15)
        {
            PlayerPrefs.SetInt("Points", 15);
        }
        else
        {
            PlayerPrefs.SetInt("Points", myPoints);
        }

        playerInventory.Deselect(false);
        playerInventory.Save();

        FindObjectOfType<Networker>().Reset();

        Network.Disconnect();
        Application.LoadLevel("menu");
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
}
