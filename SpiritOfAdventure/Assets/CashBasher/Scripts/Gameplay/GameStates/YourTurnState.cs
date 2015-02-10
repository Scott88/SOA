using UnityEngine;
using System.Collections;

public class YourTurnState : GameState
{
    CashBasherManager manager;
    NetworkedCannon yourCannon;
    CameraMan cameraMan;

    Vector3 spiritWaypoint;

    GameObject spiritButtons;

    CashBasherSpirit selectedSpirit;

    bool nextTurn = false;
    float timer = 1.0f;

    CashBasherSpiritGUI spiritGUIPushed = null;
    bool holdingSpirit = false;

    Vector3 velocityRef;

    Vector3 buttonDownPos, buttonUpPos;

    bool showButtons = true;

    public YourTurnState(CashBasherManager m, NetworkedCannon c, CameraMan cm, GameObject waypoint, GameObject buttons)
    {
        manager = m;
        yourCannon = c;
        cameraMan = cm;

        spiritWaypoint = waypoint.transform.position;

        spiritButtons = buttons;

        buttonUpPos = spiritButtons.transform.position;
        buttonDownPos = buttonUpPos;
        buttonDownPos.y -= 2.0f;
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

        showButtons = true;
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

        if (showButtons)
        {
            spiritButtons.transform.position = Vector3.SmoothDamp(spiritButtons.transform.position, buttonDownPos, ref velocityRef, 1f);
        }
        else
        {
            spiritButtons.transform.position = Vector3.SmoothDamp(spiritButtons.transform.position, buttonUpPos, ref velocityRef, 1f);
        }

        if (Input.GetMouseButtonDown(0))
        {
            GetClickedOn();
        }
        else if(Input.GetMouseButton(0))
        {
            GetHeldOn();
        }

        if(Input.GetMouseButtonUp(0))
        {
            GetReleasedOn();
        }
    }

    public void GetClickedOn()
    {
        Vector2 rayOrigin = (Vector2)(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition));
        Vector2 rayDirection = new Vector2();

        RaycastHit2D hit2d = Physics2D.Raycast(rayOrigin, rayDirection);

        if (!selectedSpirit)
        {
            if (hit2d)
            {
                NetworkedCannon cannon = hit2d.collider.GetComponent<NetworkedCannon>();

                if (cannon)
                {
                    showButtons = !cannon.Press();

                    return;
                }
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
                spiritGUIPushed = spiritGUI;
                return;
            }
        }

		rayOrigin = (Vector2)(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition));
		rayDirection = new Vector2();
		
		hit2d = Physics2D.Raycast(rayOrigin, rayDirection);

		if (hit2d)
		{		
			CashBasherSpirit spirit = hit2d.collider.GetComponent<CashBasherSpirit>();
			
			if (spirit)
			{
				holdingSpirit = true;
			}
		}
    }

    public void GetHeldOn()
    {
        if (holdingSpirit)
        {
            selectedSpirit.MoveHere(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition));
        }
        else if (spiritGUIPushed)
        {
            Vector2 rayOrigin = (Vector2)(manager.guiCamera.ScreenToWorldPoint(Input.mousePosition));
            Vector2 rayDirection = new Vector2();

            RaycastHit2D hit2d = Physics2D.Raycast(rayOrigin, rayDirection);

            if (hit2d)
            {
                CashBasherSpiritGUI spiritGUI = hit2d.collider.GetComponent<CashBasherSpiritGUI>();

                if (spiritGUI != spiritGUIPushed)
                {
                    SummonSpirit(spiritGUI);

                    return;
                }
            }
            else
            {
                SummonSpirit(spiritGUIPushed);

                return;
            }
        }
    }

    public void GetReleasedOn()
    {
        Vector2 rayOrigin = (Vector2)(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition));
        Vector2 rayDirection = new Vector2();

        RaycastHit2D hit2d = Physics2D.Raycast(rayOrigin, rayDirection);

        if (selectedSpirit)
        {
            if (hit2d)
            {
                NetworkedCannon cannon = hit2d.collider.GetComponent<NetworkedCannon>();

                if (cannon == yourCannon)
                {
                    selectedSpirit.MoveHereAndTrigger(cannon.team == 0);

                    showButtons = false;

                    holdingSpirit = false;
                    selectedSpirit = null;

                    return;
                }
            }

            selectedSpirit.MoveHereAndPoof(rayOrigin);

            holdingSpirit = false;
            selectedSpirit = null;

            return;
        }

        rayOrigin = (Vector2)(manager.guiCamera.ScreenToWorldPoint(Input.mousePosition));
        rayDirection = new Vector2();

        hit2d = Physics2D.Raycast(rayOrigin, rayDirection);

        if (hit2d)
        {
            CashBasherSpiritGUI spiritGUI = hit2d.collider.GetComponent<CashBasherSpiritGUI>();

            if (spiritGUI)
            {
                SummonSpirit(spiritGUI);

                return;
            }
        }
    }

    void SummonSpirit(CashBasherSpiritGUI spiritGUI)
    {
        if (selectedSpirit && selectedSpirit != spiritGUIPushed.spirit)
        {
            selectedSpirit.Retreat(spiritWaypoint);
        }

        selectedSpirit = spiritGUIPushed.spirit;
        selectedSpirit.Activate(spiritWaypoint);

        spiritGUIPushed = null;

        holdingSpirit = true;
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
