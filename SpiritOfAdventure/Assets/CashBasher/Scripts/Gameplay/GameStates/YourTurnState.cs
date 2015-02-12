using UnityEngine;
using System.Collections;

public class YourTurnState : GameState
{
    public CashBasherManager manager;

    public NetworkedCannon serverCannon, clientCannon;

    public GameObject serverWaypoint, clientWaypoint;

    public GameObject spiritButtons;

    public NetworkedTileSet serverTileSet, clientTileSet;

    NetworkedCannon yourCannon;

    Vector3 spiritWaypoint;

    CashBasherSpirit selectedSpirit;

    NetworkedTileSet yourTileSet;

    bool nextTurn = false;
    float timer = 1.0f;

    CashBasherSpiritGUI spiritGUIPushed = null;
    bool holdingSpirit = false;

    Vector3 velocityRef;

    Vector3 buttonDownPos, buttonUpPos;

    bool showButtons = true;

    bool airGrabbed = false, cameraGrabbed = false;
    bool cameraFocusLeft;

    Vector3 lastTouchPos;

    void Start()
    {
        if (Network.isServer)
        {
            yourCannon = serverCannon;
            yourTileSet = serverTileSet;
            spiritWaypoint = serverWaypoint.transform.position;
        }
        else
        {
            yourCannon = clientCannon;
            yourTileSet = clientTileSet;
            spiritWaypoint = clientWaypoint.transform.position;
        }

        buttonUpPos = spiritButtons.transform.position;
        buttonDownPos = buttonUpPos;
        buttonDownPos.y -= 2.0f;
    }

    public override void Prepare()
    {
        //if (Network.isServer)
        //{
        //    cameraMan.FollowPosition(new Vector3(4.5f, 1f, 0f));
        //}
        //else
        //{
        //    cameraMan.FollowPosition(new Vector3(-4.5f, 1f, 0f));
        //}

        //cameraMan.ZoomTo(7f);

        //yourCannon.Activate();

        //showButtons = true;

        manager.StartCoroutine(Preshow());
    }

    IEnumerator Preshow()
    {
        if (manager.HasEffects(Network.isServer))
        {
            if (Network.isServer)
            {
                manager.cameraMan.FollowPosition(new Vector3(-10f, 0f, 0f));
            }
            else
            {
                manager.cameraMan.FollowPosition(new Vector3(10f, 0f, 0f));
            }

            manager.cameraMan.ZoomTo(5f);

            yield return new WaitForSeconds(2.0f);

            yourTileSet.TickDebuffs();

            yield return new WaitForSeconds(1.0f);
        }

        if (Network.isServer)
        {
            manager.cameraMan.FollowPosition(new Vector3(4.5f, 1f, 0f));
            cameraFocusLeft = false;
        }
        else
        {
            manager.cameraMan.FollowPosition(new Vector3(-4.5f, 1f, 0f));
            cameraFocusLeft = true;
        }

        manager.cameraMan.ZoomTo(7f);

        yourCannon.Activate();

        showButtons = true;
    }

    public void ReadyNextTurn()
    {
        nextTurn = true;
        manager.networkView.RPC("UpdateEffectStatus", RPCMode.Others);
    }

    public override void UpdateState()
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
        Vector2 playerRayOrigin = (Vector2)(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition));
        Vector2 playerRayDirection = new Vector2();

        RaycastHit2D playerHit2d = Physics2D.Raycast(playerRayOrigin, playerRayDirection);

        if (!selectedSpirit)
        {
            if (playerHit2d)
            {
                NetworkedCannon cannon = playerHit2d.collider.GetComponent<NetworkedCannon>();

                if (cannon)
                {
                    if (showButtons)
                    {
                        showButtons = !cannon.Press();
                    }
                    else
                    {
                        cannon.Press();
                    }

                    return;
                }
            }
        }

        Vector2 rayOrigin = (Vector2)(manager.guiCamera.ScreenToWorldPoint(Input.mousePosition));
        Vector2 rayDirection = new Vector2();

        RaycastHit2D hit2d = Physics2D.Raycast(rayOrigin, rayDirection);

        if (hit2d && showButtons)
        {
            CashBasherSpiritGUI spiritGUI = hit2d.collider.GetComponent<CashBasherSpiritGUI>();

            if (spiritGUI)
            {
                spiritGUIPushed = spiritGUI;
                return;
            }
        }

        if (playerHit2d)
		{
            CashBasherSpirit spirit = playerHit2d.collider.GetComponent<CashBasherSpirit>();
			
			if (spirit)
			{
				holdingSpirit = true;
                return;
			}
		}

        if(selectedSpirit && yourTileSet.IsInside(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition)))
        {
            return;
        }

        airGrabbed = true;
        lastTouchPos = manager.playerCamera.ScreenToWorldPoint(Input.mousePosition);
        manager.cameraMan.enabled = false;
    }

    public void GetHeldOn()
    {
        if (airGrabbed)
        {
            Vector3 nextPos = manager.playerCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector3 translation = new Vector3(lastTouchPos.x - nextPos.x, 0f);

            if (cameraFocusLeft && translation.x + manager.playerCamera.transform.position.x < -4.5f)
            {
                translation.x = -4.5f - manager.playerCamera.transform.position.x;
            }
            else if (!cameraFocusLeft && translation.x + manager.playerCamera.transform.position.x > 4.5f)
            {
                translation.x = 4.5f - manager.playerCamera.transform.position.x;
            }

            manager.playerCamera.transform.Translate(translation);

            if (Mathf.Abs(translation.x) > 0.5f)
            {
                cameraGrabbed = true;
            }
        }

        if (!cameraGrabbed)
        {
            if (spiritGUIPushed)
            {
                Vector2 rayOrigin = (Vector2)(manager.guiCamera.ScreenToWorldPoint(Input.mousePosition));
                Vector2 rayDirection = new Vector2();

                RaycastHit2D hit2d = Physics2D.Raycast(rayOrigin, rayDirection);

                if (hit2d)
                {
                    CashBasherSpiritGUI spiritGUI = hit2d.collider.GetComponent<CashBasherSpiritGUI>();

                    if (spiritGUI != spiritGUIPushed)
                    {
                        SummonSpirit(spiritGUI, true);

                        return;
                    }
                }
                else
                {
                    SummonSpirit(spiritGUIPushed, true);

                    return;
                }
            }

            if (holdingSpirit)
            {
                selectedSpirit.MoveHere(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition) + Vector3.up * 2f);
            }

            if (selectedSpirit)
            {
                if (yourTileSet.IsInside(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition)))
                {
                    selectedSpirit.healAOE.SetActive(true);
                    selectedSpirit.healAOE.transform.position = yourTileSet.CenterOn(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition));
                    selectedSpirit.healAOE.transform.position += Vector3.back * 2f;
                }
                else
                {
                    selectedSpirit.healAOE.SetActive(false);
                }
            }
        }
    }

    public void GetReleasedOn()
    {
        if (airGrabbed)
        {
            manager.cameraMan.enabled = true;
            airGrabbed = false;
        }

        if (cameraGrabbed)
        {
            if (cameraFocusLeft && manager.playerCamera.transform.position.x > 0f)
            {
                manager.cameraMan.FollowPosition(new Vector3(4.5f, 1f, 0f));
                cameraFocusLeft = false;
                manager.networkView.RPC("FocusCamera", RPCMode.Others, cameraFocusLeft);
            }
            else if (!cameraFocusLeft && manager.playerCamera.transform.position.x < 0f)
            {
                manager.cameraMan.FollowPosition(new Vector3(-4.5f, 1f, 0f));
                cameraFocusLeft = true;
                manager.networkView.RPC("FocusCamera", RPCMode.Others, cameraFocusLeft);
            }

            cameraGrabbed = false;
        }
        else
        {
            Vector2 rayOrigin = (Vector2)(manager.guiCamera.ScreenToWorldPoint(Input.mousePosition));
            Vector2 rayDirection = new Vector2();

            RaycastHit2D hit2d = Physics2D.Raycast(rayOrigin, rayDirection);

            if (hit2d && spiritGUIPushed)
            {
                CashBasherSpiritGUI spiritGUI = hit2d.collider.GetComponent<CashBasherSpiritGUI>();

                if (spiritGUI)
                {
                    if (selectedSpirit == spiritGUI.spirit)
                    {
                        selectedSpirit.Retreat(spiritWaypoint);
                        selectedSpirit = null;
                        spiritGUIPushed = null;
                    }
                    else
                    {
                        SummonSpirit(spiritGUI, false);
                    }

                    return;
                }
            }

            rayOrigin = (Vector2)(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition));
            rayDirection = new Vector2();

            hit2d = Physics2D.Raycast(rayOrigin, rayDirection);

            if (selectedSpirit)
            {
                if (hit2d)
                {
                    NetworkedCannon cannon = hit2d.collider.GetComponent<NetworkedCannon>();

                    if (cannon == yourCannon && cannon.CanApplyBuff(selectedSpirit.type))
                    {
                        selectedSpirit.MoveHereAndTrigger(cannon.team == 0);

                        showButtons = false;

                        holdingSpirit = false;
                        selectedSpirit = null;

                        return;
                    }
                }

                if (yourTileSet.IsInside(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition)))
                {
                    selectedSpirit.healAOE.SetActive(false);
                    selectedSpirit.MoveHereAndHeal(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition), Network.isServer);

                    showButtons = false;

                    holdingSpirit = false;
                    selectedSpirit = null;

                    return;
                }

                selectedSpirit.MoveHereAndPoof(rayOrigin);

                holdingSpirit = false;
                selectedSpirit = null;

                return;
            }
        }
    }

    void SummonSpirit(CashBasherSpiritGUI spiritGUI, bool holding)
    {
        if (selectedSpirit == spiritGUI.spirit)
        {
            return;
        }

        if (selectedSpirit && selectedSpirit != spiritGUIPushed.spirit)
        {
            selectedSpirit.Retreat(spiritWaypoint);
        }

        selectedSpirit = spiritGUIPushed.spirit;
        selectedSpirit.Activate(spiritWaypoint);

        spiritGUIPushed = null;

        holdingSpirit = holding;
    }

    public override void End()
    {
        yourCannon.Deactivate();

        nextTurn = false;
        timer = 1.0f;
    }
}
