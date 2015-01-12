using UnityEngine;
using System.Collections;

enum NetworkAction
{
    NS_NULL,
    NS_SERVER,
    NS_CONNECT
}

public class Networker : MonoBehaviour {

    public TextMesh debugMessage;

    private NetworkAction action = NetworkAction.NS_NULL;

    private HostData[] hostList;

    private bool readyToConnect = false;

	// Use this for initialization
	void Start ()
    {
	
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
        if (action == NetworkAction.NS_NULL)
        {
            if (GUI.Button(new Rect(Screen.width * (0.125f), Screen.height * (0.4f), Screen.width * (1f / 4f), Screen.height * (1f / 6f)),
                           "Host Server"))
            {
                debugMessage.text = "Creating a Server...";
                Network.InitializeServer(3, 25311, !Network.HavePublicAddress());
				MasterServer.ipAddress = "10.10.10.181";
                MasterServer.port = 23466;
                MasterServer.RegisterHost("TestingTheTestOfTests", "SpiritOfTestventure");
                action = NetworkAction.NS_SERVER;
            }
        }

        if (GUI.Button(new Rect(Screen.width * (0.625f), Screen.height * (0.4f), Screen.width * (1f / 4f), Screen.height * (1f / 6f)),
                           "Connect to Server"))
        {         
            RefreshHostList();
            action = NetworkAction.NS_CONNECT;
        }
    }

    void OnServerInitialized()
    {
        debugMessage.text = "Server initialized and ready";
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        debugMessage.text = "Someone has connected!";      
    }

    void OnFailedToConnect(NetworkConnectionError info)
    {
        debugMessage.text = "Something went wrong...";  
    }

    private void RefreshHostList()
    {
		MasterServer.RequestHostList("TestingTheTestOfTests");
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
