//#define HOME_LAN
//#define RABBIT_HOLE_SERVER

using UnityEngine;
using System.Collections;

public class ServerMenu : MonoBehaviour
{
    public GUISkin mySkin;

    public TextMesh display;

    public GameObject minigameMenu;

    public Camera mainCamera;

    private bool readyToStart = false;
    private bool serverFailed = false;

    private float startTimer = 1.0f;

    // Use this for initialization
    void Start()
    {
#if HOME_LAN
        MasterServer.ipAddress = "192.168.2.18";
        Network.natFacilitatorIP = "192.168.2.18";
        Network.natFacilitatorPort = 50005;
#elif RABBIT_HOLE_SERVER
        MasterServer.ipAddress = "rabbitholestudios.ca";
        Network.natFacilitatorIP = "rabbitholestudios.ca";
        Network.natFacilitatorPort = 50005;
#else
        MasterServer.ipAddress = "10.10.10.181";
#endif


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
                FindObjectOfType<NetworkedLevelLoader>().LoadLevel("MiniGame");
            }

            display.text = "Player found!\nGame starts in:\n" + startTimer.ToString("0.#");
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

        MasterServer.RegisterHost("SorryUnityIWillGiveYouBackYourServerWhenTheBlizzardClears", "SpiritOfAdventure", myPoints.ToString());
    }

    void OnGUI()
    {
        GUI.skin = mySkin;

        if (!readyToStart)
        {
            if (GUI.Button(new Rect(Screen.width * (0.8f), Screen.height * (0.8f), Screen.width * (0.18f), Screen.height * (0.18f)),
                               "Back"))
            {
                mainCamera.transform.position = new Vector3(0f, 0f, -10f);

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
