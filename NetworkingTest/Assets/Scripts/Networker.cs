using UnityEngine;
using System.Collections;

public class Networker : MonoBehaviour {

    public TextMesh debugMessage;

    private HostData[] hostList;

    private bool readyToConnect = false;
    private bool readyToStart = false;

    private int levelPrefix = 0;

    void Awake()
    {
        DontDestroyOnLoad(this);
        networkView.group = 1;
    }

	// Use this for initialization
	void Start ()
    {
		MasterServer.ipAddress = "10.10.10.181";
		MasterServer.port = 23466;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (readyToConnect)
        {
            if (hostList.Length >= 1)
            {
                debugMessage.text = "Connecting to server";
                Network.Connect(hostList[0]);
                readyToConnect = false;
            }
        }
	}

    void OnGUI()
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            if (GUI.Button(new Rect(Screen.width * (0.125f), Screen.height * (0.4f), Screen.width * (1f / 4f), Screen.height * (1f / 6f)),
                           "Host Server"))
            {
                debugMessage.text = "Creating a Server...";
                Network.InitializeServer(3, 25311, !Network.HavePublicAddress());
                MasterServer.RegisterHost("TestingTheTestOfTests", "SpiritOfTestventure");
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
                networkView.RPC("LoadNetworkedLevel", RPCMode.AllBuffered);
            }
        }
    }

    [RPC]
    void LoadNetworkedLevel()
    {
        Network.SetSendingEnabled(0, false);

        Network.isMessageQueueRunning = false;

        Network.SetLevelPrefix(levelPrefix++);
        Application.LoadLevel("game");

        Network.isMessageQueueRunning = true;
        Network.SetSendingEnabled(0, true);

        gameObject.SetActive(false);
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
        if (Network.peerType == NetworkPeerType.Client)
        {
            MasterServer.RequestHostList("TestingTheTestOfTests");
        }
        else
        {
            debugMessage.text = "Could not retrieve host list from master server!";
        }
    }

    void OnFailedToConnectToMasterServer(NetworkConnectionError info)
    {
        debugMessage.text = "Could not register with the master server!";
    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            hostList = MasterServer.PollHostList();
            readyToConnect = true;
        }
    }
}
