using UnityEngine;
using System.Collections;

public class YourTurnState : GameState
{
    CashBasherManager manager;
    NetworkedCannon yourCannon;
    CameraMan cameraMan;

    Vector3 spiritWaypoint;

    CashBasherSpirit selectedSpirit;

    bool nextTurn = false;
    float timer = 1.0f;

    public YourTurnState(CashBasherManager m, NetworkedCannon c, CameraMan cm, GameObject waypoint)
    {
        manager = m;
        yourCannon = c;
        cameraMan = cm;

        spiritWaypoint = waypoint.transform.position;
    }

    public void Prepare()
    {
        if (Network.isServer)
        {
            cameraMan.FollowPosition(new Vector3(4.5f, 1f, 0f));
        }
        else
        {
            cameraMan.FollowPosition(new Vector3(-4.5f, 1f, 0f));
        }

        cameraMan.ZoomTo(7f);

        yourCannon.Activate();
    }

    public void ReadyNextTurn()
    {
        nextTurn = true;
    }

    public void Update()
    {
        if (nextTurn)
        {
            timer -= Time.deltaTime;

            if (timer <= 0.0f)
            {
                manager.networkView.RPC("SwitchToState", RPCMode.Others, (int)GamePhase.GP_YOUR_TURN);
                manager.SwitchToState((int)GamePhase.GP_THEIR_TURN);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            GetClickedOn();
        }
        else if(Input.GetMouseButton(0))
        {
            GetHeldOn();
        }

        if(Input.GetMouseButtonDown(0))
        {
            GetReleasedOn();
        }
    }

    public void GetClickedOn()
    {
        Vector2 rayOrigin = (Vector2)(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition));
        Vector2 rayDirection = new Vector2();

        RaycastHit2D hit2d = Physics2D.Raycast(rayOrigin, rayDirection);

        if (hit2d)
        {
            NetworkedCannon cannon = hit2d.collider.GetComponent<NetworkedCannon>();

            if (cannon)
            {
                cannon.Press();
                return;
            }
        }

        rayOrigin = (Vector2)(manager.guiCamera.ScreenToWorldPoint(Input.mousePosition));
        rayDirection = new Vector2();

        hit2d = Physics2D.Raycast(rayOrigin, rayDirection);

        if (hit2d)
        {
            CashBasherSpiritGUI spiritGUI = hit2d.collider.GetComponent<CashBasherSpiritGUI>();

            if (spiritGUI)
            {
                if (selectedSpirit && selectedSpirit != spiritGUI.spirit)
                {
                    selectedSpirit.Retreat(spiritWaypoint);
                }

                selectedSpirit = spiritGUI.spirit;
                selectedSpirit.Activate(spiritWaypoint);
                return;
            }
        }
    }

    public void GetHeldOn()
    {

    }

    public void GetReleasedOn()
    {

    }

    public void OnGUI()
    {

    }

    public void End()
    {
        yourCannon.Deactivate();

        nextTurn = false;
        timer = 1.0f;
    }
}
