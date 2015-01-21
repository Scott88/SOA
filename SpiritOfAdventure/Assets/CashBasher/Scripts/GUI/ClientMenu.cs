using UnityEngine;
using System.Collections;

public class ClientMenu : MonoBehaviour
{
    public GUISkin mySkin;
    public TextMesh display;

    public GameObject minigameMenu;

    private bool clientFailed = false;

    void OnGUI()
    {
        GUI.skin = mySkin;

        if (GUI.Button(new Rect(Screen.width * (0.8f), Screen.height * (0.8f), Screen.width * (0.18f), Screen.height * (0.18f)),
                           "Back"))
        {
            gameObject.SetActive(false);
            minigameMenu.SetActive(true);
        }

        if (clientFailed)
        {
            if (GUI.Button(new Rect(Screen.width * (0.41f), Screen.height * (0.8f), Screen.width * (0.18f), Screen.height * (0.18f)),
                           "Retry"))
            {
                clientFailed = false;

                SearchForServers();
            }
        }
    }

    public void SearchForServers()
    {
        display.text = "Looking for servers...";
        MasterServer.RequestHostList("CashBasher");
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
                display.text = "No valid server to connect to!";
                clientFailed = true;
            }
            else
            {
                display.text = "Found a server! Connecting now...";
                Network.Connect(hostList[closestServer]);
            }
        }
    }

    void OnConnectedToServer()
    {
        display.text = "Connected! Waiting for the game to start...";
    }

    void OnFailedToConnect(NetworkConnectionError info)
    {
        display.text = "Something went wrong...";
        clientFailed = true;
    }
}
