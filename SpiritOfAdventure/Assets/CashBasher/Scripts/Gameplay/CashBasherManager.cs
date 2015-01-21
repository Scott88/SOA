﻿using UnityEngine;
using System.Collections;

public class CashBasherManager : MonoBehaviour
{
    public float startTimer = 2.0f;
    public float buildTimer = 30.0f;

    public Camera playerCamera, guiCamera;

    public CameraMan cameraMan;

    public TileSet serverSet, clientSet;

    public GameObject spawnIndicator;

    public int myTeam;

    private GameState state = null;
    private GamePhase currentPhase = GamePhase.GP_STARTING;

    private ArrayList blocks;

    private NetworkedLevelLoader loader;

    private BuildState buildState;

    enum GamePhase
    {
        GP_STARTING = 0,
        GP_BUILD = 1,
        GP_YOUR_TURN = 2,
        GP_THEIR_TURN = 3,
        GP_OVER = 4
    }

    void Start()
    {
        loader = FindObjectOfType<NetworkedLevelLoader>();

        blocks = new ArrayList();

        if (Network.isServer)
        {
            myTeam = 0;
            cameraMan.FollowPosition(new Vector3(-10f, 0f, 0f));
        }
        else if (Network.isClient)
        {
            myTeam = 1;
            cameraMan.FollowPosition(new Vector3(10f, 0f, 0f)); 
        }
        
        cameraMan.ZoomTo(4f);

        buildState = new BuildState(this, myTeam, myTeam == 0 ? serverSet : clientSet, spawnIndicator);
    }

    public void AddBlock(Breakable b)
    {
        blocks.Add(b);
    }

    void Update()
    {
        if (currentPhase == GamePhase.GP_STARTING)
        {
            startTimer -= Time.deltaTime;

            if (startTimer <= 0f)
            {
                if (loader.IsReady())
                {
                    //SwitchToState(buildState, GamePhase.GP_BUILD);

                    networkView.RPC("NetSwitchToState", RPCMode.All, (int)GamePhase.GP_BUILD);
                }
            }
        }
        if (currentPhase == GamePhase.GP_BUILD)
        {
            buildTimer -= Time.deltaTime;
        }

        if (state != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                state.GetClickedOn();
            }
            else if (Input.GetMouseButton(0))
            {
                state.GetHeldOn();
            }

            if (Input.GetMouseButtonUp(0))
            {
                state.GetReleasedOn();
            }
        }
    }

    void SwitchToState(GameState s, GamePhase phase)
    {
        currentPhase = phase;

        if (state != null)
        {
            state.End();
        }

        state = s;

        if (state != null)
        {
            state.Prepare();
        }
    }

    [RPC]
    void NetSwitchToState(int phase)
    {
        currentPhase = (GamePhase)(phase);


        if (state != null)
        {
            state.End();
        }

        switch (currentPhase)
        {
            case GamePhase.GP_STARTING:
                state = null;
                break;
            case GamePhase.GP_BUILD:
                state = buildState;
                break;
        }

        if (state != null)
        {
            state.Prepare();
        }
    }

    
}