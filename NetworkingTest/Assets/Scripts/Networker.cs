using UnityEngine;
using System.Collections;

public class Networker : MonoBehaviour {

    public TextMesh debugMessage;

    public bool showOptions { get; set; }

    private bool readyToStart = false;  

	// Use this for initialization
	void Start ()
    {
        showOptions = true;

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
                    Network.InitializeServer(1, 25311, !Network.HavePublicAddress());

                    if (!PlayerPrefs.HasKey("Blocks"))
                    {
                        PlayerPrefs.SetInt("Blocks", 3);
                    }

                    int myPoints = PlayerPrefs.GetInt("Blocks") * 5;

                    MasterServer.RegisterHost("TestingTheTestOfTests", "SpiritOfTestventure", myPoints.ToString());
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
                    FindObjectOfType<LevelLoader>().LoadLevel();
                }
            }
        }
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

            if (!PlayerPrefs.HasKey("Blocks"))
            {
                PlayerPrefs.SetInt("Blocks", 3);
            }

            int myPoints = PlayerPrefs.GetInt("Blocks") * 5;
            int closestServer = -1;
            int closestPoints = -100;

            for (int j = 0; j < hostList.Length; j++)
            {
                if (hostList[j].connectedPlayers == 1)
                {
                    int hostPoints = int.Parse(hostList[j].comment);

                    if (Mathf.Abs(myPoints - closestPoints) > Mathf.Abs(myPoints - hostPoints))
                    {
                        closestPoints = hostPoints;
                        closestServer = j;
                    }
                }
            }

            if (closestServer == -1)
            {
                debugMessage.text = "No valid server to connect to!";
            }
            else
            {
                debugMessage.text = "Connecting to server with " + hostList[closestServer].comment + " points...";
                Network.Connect(hostList[closestServer]);
            }
        }
    }
}
