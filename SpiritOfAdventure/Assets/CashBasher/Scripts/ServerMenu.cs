using UnityEngine;
using System.Collections;

public class ServerMenu : MonoBehaviour
{
    public GUISkin mySkin;

    public TextMesh display;

    public GameObject minigameMenu;

    public NetworkedLevelLoader levelLoader;

    private bool readyToStart = false;
    private bool serverFailed = false;

    private float startTimer = 3.0f;

    // Use this for initialization
    void Start()
    {
        MasterServer.ipAddress = "10.10.10.181";
        MasterServer.port = 23466;

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (readyToStart)
        {
            startTimer -= Time.deltaTime;

            if (startTimer <= 0.0f)
            {
                levelLoader.LoadLevel("MiniGame");
            }

            display.text = "Player found! Game starts in : " + startTimer.ToString("g");
        }
    }

    public void CreateServer()
    {
        display.text = "Initializing the server...";
        Network.InitializeServer(1, 25311, !Network.HavePublicAddress());

        if (!PlayerPrefs.HasKey("Blocks"))
        {
            PlayerPrefs.SetInt("Blocks", 3);
        }

        int myPoints = PlayerPrefs.GetInt("Blocks") * 5;

        MasterServer.RegisterHost("CashBasher", "SpiritOfAdventure", myPoints.ToString());
    }

    void OnGUI()
    {
        GUI.skin = mySkin;

        if (!readyToStart)
        {
            if (GUI.Button(new Rect(Screen.width * (0.8f), Screen.height * (0.8f), Screen.width * (0.18f), Screen.height * (0.18f)),
                               "Back"))
            {
                if (Network.isServer)
                {
                    Network.Disconnect();
                }

                gameObject.SetActive(false);
                minigameMenu.SetActive(true);
            }
        }

        if (serverFailed)
        {
            if (GUI.Button(new Rect(Screen.width * (0.41f), Screen.height * (0.8f), Screen.width * (0.18f), Screen.height * (0.18f)),
                           "Retry"))
            {
                serverFailed = false;

                if (Network.isServer)
                {
                    Network.Disconnect();
                }

                CreateServer();
            }
        }
    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.RegistrationSucceeded)
        {
            display.text = "Waiting for players...";
        }
        else if (msEvent != MasterServerEvent.RegistrationSucceeded && msEvent != MasterServerEvent.HostListReceived)
        {
            display.text = "Could not connect to the master server :(";
            serverFailed = true;
        }
    }

    void OnFailedToConnectToMasterServer(NetworkConnectionError info)
    {
        display.text = "Could not register with the master server!";
        serverFailed = true;
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        readyToStart = true;
    }

    
}
