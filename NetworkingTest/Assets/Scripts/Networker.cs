using UnityEngine;
using System.Collections;

public class Networker : MonoBehaviour {

    public TextMesh debugMessage;

    private static bool created = false;

    private bool showOptions = true;

    private bool readyToStart = false;

    private int levelPrefix = 0;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this);
            networkView.group = 1;

            created = true;
        }
        else
        {
            showOptions = true;
            DestroyImmediate(gameObject);
        }
    }

	// Use this for initialization
	void Start ()
    {
		MasterServer.ipAddress = "10.10.10.181";
		MasterServer.port = 23466;
	}

    void OnGUI()
    {
        if (showOptions)
        {
            if (Network.peerType == NetworkPeerType.Disconnected)
            {
                if (GUI.Button(new Rect(Screen.width * (0.125f), Screen.height * (0.4f), Screen.width * (1f / 4f), Screen.height * (1f / 6f)),
                               "Host Server"))
                {
                    debugMessage.text = "Creating a Server...";
                    Network.InitializeServer(3, 25311, !Network.HavePublicAddress());

                    if (!PlayerPrefs.HasKey("Points"))
                    {
                        PlayerPrefs.SetInt("Points", 15);  
                    }

                    MasterServer.RegisterHost("TestingTheTestOfTests", "SpiritOfTestventure", PlayerPrefs.GetInt("Points").ToString());
                }

                if (GUI.Button(new Rect(Screen.width * (0.625f), Screen.height * (0.4f), Screen.width * (1f / 4f), Screen.height * (1f / 6f)),
                               "Connect to Server"))
                {
                    RefreshHostList();
                }
            }

            if (readyToStart)
            {
                if (GUI.Button(new Rect(Screen.width * (0.375f), Screen.height * (0.4f), Screen.width * (1f / 4f), Screen.height * (1f / 6f)),
                               "Load Game"))
                {
                    MasterServer.UnregisterHost();
                    networkView.RPC("LoadNetworkedLevel", RPCMode.AllBuffered);
                }
            }
        }
    }

    [RPC]
    void LoadNetworkedLevel()
    {
        Network.SetSendingEnabled(0, false);
        Network.isMessageQueueRunning = false;

        Network.SetLevelPrefix(++levelPrefix);
        Application.LoadLevel("game");

        Network.isMessageQueueRunning = true;
        Network.SetSendingEnabled(0, true);

        debugMessage.text = "";
        showOptions = false;
    }

    void OnServerInitialized()
    {
        debugMessage.text = "Server initialized and ready";
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        readyToStart = true;
        debugMessage.text = "Number of players connected: " + Network.connections.Length;
    }

    void OnConnectedToServer()
    {
        debugMessage.text = "Connection successful!";
    }

    void OnFailedToConnect(NetworkConnectionError info)
    {
        debugMessage.text = "Something went wrong...";  
    }

    private void RefreshHostList()
    {
        MasterServer.RequestHostList("TestingTheTestOfTests");
    }

    void OnFailedToConnectToMasterServer(NetworkConnectionError info)
    {
        debugMessage.text = "Could not register with the master server!";
    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            HostData[] hostList = MasterServer.PollHostList();

            if (!PlayerPrefs.HasKey("Points"))
            {
                PlayerPrefs.SetInt("Points", 15);
            }

            int myPoints = PlayerPrefs.GetInt("Points");
            int closestServer = -1;
            int closestPoints = -100;

            for (int j = 0; j < hostList.Length; j++)
            {
                int hostPoints = int.Parse(hostList[j].comment);

                if (Mathf.Abs(myPoints - closestPoints) > Mathf.Abs(myPoints - hostPoints))
                {
                    closestPoints = hostPoints;
                    closestServer = j;
                }
            }

            debugMessage.text = "Connecting to server with " + hostList[closestServer].comment + " points...";
            Network.Connect(hostList[closestServer]);
        }
    }

    public void Reset()
    {
        showOptions = true;
        readyToStart = false;

        debugMessage.text = "";
    }
}
