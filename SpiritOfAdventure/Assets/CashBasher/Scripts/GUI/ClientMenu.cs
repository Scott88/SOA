﻿using UnityEngine;
using System.Collections;

public class ClientMenu : MonoBehaviour
{
    public GUISkin mySkin;
    public TextMesh display;

    public GameObject minigameMenu;

    public Camera mainCamera;

    public BlockEditor editor;

    public float recheckDelay = 5f;

    private float timer;

    private bool clientFailed = false;

    private bool hostListRequested = false;

    private int otherCastleWorth;

    void OnGUI()
    {
        GUI.skin = mySkin;

        if (GUI.Button(new Rect(Screen.width * (0.8f), Screen.height * (0.8f), Screen.width * (0.18f), Screen.height * (0.18f)),
                           "Back"))
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            mainCamera.transform.position = new Vector3(0f, 0f, -10f);
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

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f && !hostListRequested)
        {
            MasterServer.RequestHostList(ServerMenu.GAME_NAME);
            hostListRequested = true;
        }
    }

    public void SearchForServers()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        display.text = "Looking for servers...";
        timer = 0f;
    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            HostData[] hostList = MasterServer.PollHostList();

            int myPoints = editor.GetCastleWorth();
            int closestServer = -1;
            int closestPoints = 10000;

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

            if (closestServer != -1)
            {
                display.text = "Found a server!\nConnecting now...";
                otherCastleWorth = closestPoints;
                Network.Connect(hostList[closestServer]);
            }

            timer = recheckDelay;

            hostListRequested = false;
        }
    }

    void OnConnectedToServer()
    {
        display.text = "Connected!\nWaiting for the\ngame to start...";
        NetworkedLevelLoader loader = FindObjectOfType<NetworkedLevelLoader>();

        loader.othersCastleWorth = otherCastleWorth;
        loader.GetComponent<NetworkView>().RPC("ReceiveOtherCastleWorth", RPCMode.Others, editor.GetCastleWorth());
    }

    void OnFailedToConnect(NetworkConnectionError info)
    {
        display.text = "Something went wrong...";
        clientFailed = true;
    }
}
