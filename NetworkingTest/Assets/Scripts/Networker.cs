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

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (action == NetworkAction.NS_NULL)
        {

        }
	}

    void OnGUI()
    {
        if (action == NetworkAction.NS_NULL)
        {
            if (GUI.Button(new Rect(Screen.width * (0.125f), Screen.height * (0.4f), Screen.width * (1f / 4f), Screen.height * (1f / 6f)),
                           "Host Server"))
            {
                Network.InitializeServer(1, 25311, !Network.HavePublicAddress());
                action = NetworkAction.NS_SERVER;
                debugMessage.text = "Created a Server";
            }

            if (GUI.Button(new Rect(Screen.width * (0.625f), Screen.height * (0.4f), Screen.width * (1f / 4f), Screen.height * (1f / 6f)),
                           "Connect to Server"))
            {
                debugMessage.text = "Connecting to server";
                Network.Connect("192.168.56.1", 25311);
                action = NetworkAction.NS_CONNECT;
            }
        }
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        if (action == NetworkAction.NS_SERVER)
        {
            debugMessage.text = "Someone has connected!";
        }
    }

    void OnFailedToConnect(NetworkConnectionError info)
    {
        debugMessage.text = "Something went wrong...";  
    }
}
