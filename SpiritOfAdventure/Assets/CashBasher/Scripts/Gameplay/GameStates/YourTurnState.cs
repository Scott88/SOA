﻿using UnityEngine;
using System.Collections;

public class YourTurnState : GameState
{
    public float turnLength = 20f;

    public CashBasherManager manager;

    public NetworkedCannon serverCannon, clientCannon;

    public GameObject spiritButtons;

    public NetworkedTileSet serverTileSet, clientTileSet;

    public TextMesh timerText;

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

    bool spiritUsed = false;
    bool showButtons = true;

    bool airGrabbed = false, cameraGrabbed = false;
    bool cameraFocusLeft;

    bool turnStarted;

    bool cannonFired;

    Vector3 lastTouchPos;

    private float moveLeftPos, moveRightPos;

    void Start()
    {
        if (Network.isServer)
        {
            yourCannon = serverCannon;
            yourTileSet = serverTileSet;
            spiritWaypoint = manager.serverWaypoint.transform.position;
        }
        else
        {
            yourCannon = clientCannon;
            yourTileSet = clientTileSet;
            spiritWaypoint = manager.clientWaypoint.transform.position;
        }

        buttonUpPos = spiritButtons.transform.position;
        buttonDownPos = buttonUpPos;
        buttonDownPos.y -= 2.5f;

        moveLeftPos = manager.clientCamFocus.transform.position.x - manager.playerCamera.orthographicSize;
        moveRightPos = manager.serverCamFocus.transform.position.x + manager.playerCamera.orthographicSize;

        timerText.text = "";
    }

    public override void Prepare()
    {
        manager.StartCoroutine(Preshow());
    }

    IEnumerator Preshow()
    {
        yourCannon.GetComponent<Collider2D>().enabled = false;

        nextTurn = false;

        turnStarted = false;
        cannonFired = false;

        timer = turnLength;

        if (manager.HasEffects(Network.isServer))
        {
            if (Network.isServer)
            {
                manager.cameraMan.FollowWaypoint(manager.serverTileFocus);
            }
            else
            {
                manager.cameraMan.FollowWaypoint(manager.clientTileFocus);
            }

            manager.cameraMan.ZoomTo(5f);

            yield return new WaitForSeconds(2.0f);

            yourTileSet.TickDebuffs();

            yield return new WaitForSeconds(1.0f);
        }

        if (Network.isServer == manager.startLookingAtTarget)
        {
            manager.cameraMan.FollowWaypoint(manager.clientCamFocus);
            cameraFocusLeft = false;
        }
        else
        {
            manager.cameraMan.FollowWaypoint(manager.serverCamFocus);
            cameraFocusLeft = true;
        }

        manager.cameraMan.ZoomTo(7f);

        yourCannon.Activate();

        showButtons = true;
        spiritUsed = false;

        turnStarted = true;
    }

    public void ReadyNextTurn()
    {
        timer = 1f;
        manager.GetComponent<NetworkView>().RPC("UpdateEffectStatus", RPCMode.Others);
        nextTurn = true;
    }

    public override void UpdateState()
    {
        if (showButtons)
        {
            spiritButtons.transform.position = Vector3.SmoothDamp(spiritButtons.transform.position, buttonDownPos, ref velocityRef, 1f);
        }
        else
        {
            spiritButtons.transform.position = Vector3.SmoothDamp(spiritButtons.transform.position, buttonUpPos, ref velocityRef, 1f);
        }

        timer -= Time.deltaTime;

        if (nextTurn)
        {
            if (timer <= 0.0f)
            {
                manager.GetComponent<NetworkView>().RPC("SwitchToState", RPCMode.Others, (int)GamePhase.GP_YOUR_TURN);
                manager.SwitchToState((int)GamePhase.GP_THEIR_TURN);
            }
        }
        else if (!cannonFired)
        {
            if (timer > 0f)
            {
                if (!manager.paused && turnStarted)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        GetClickedOn();
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        GetHeldOn();
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        GetReleasedOn();
                    }
                }

                timerText.text = timer.ToString("0");
            }
            else
            {
                timerText.text = "";
                yourCannon.ForceFire();
                cannonFired = true;
            }
        }
        else
        {
            timerText.text = "";
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
                CannonClickListener cannonListener = playerHit2d.collider.GetComponent<CannonClickListener>();

                if (cannonListener)
                {
                    if (showButtons)
                    {
                        cannonFired = cannonListener.cannon.Press();
                        showButtons = !cannonFired;
                    }
                    else
                    {
                        cannonFired = cannonListener.cannon.Press();
                    }

                    return;
                }
            }
        }

        Vector2 rayOrigin = (Vector2)(manager.guiCamera.ScreenToWorldPoint(Input.mousePosition));
        Vector2 rayDirection = new Vector2();

        RaycastHit2D hit2d = Physics2D.Raycast(rayOrigin, rayDirection);

        if (hit2d && !spiritUsed)
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

            if (cameraFocusLeft && translation.x + manager.playerCamera.transform.position.x < manager.serverCamFocus.transform.position.x)
            {
                translation.x = manager.serverCamFocus.transform.position.x - manager.playerCamera.transform.position.x;
            }
            else if (!cameraFocusLeft && translation.x + manager.playerCamera.transform.position.x > manager.clientCamFocus.transform.position.x)
            {
                translation.x = manager.clientCamFocus.transform.position.x - manager.playerCamera.transform.position.x;
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
                    selectedSpirit.healAOE.transform.localScale = new Vector3(yourTileSet.blockSize, yourTileSet.blockSize);
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
            if (cameraFocusLeft && manager.playerCamera.transform.position.x > moveRightPos)
            {
                manager.cameraMan.FollowWaypoint(manager.clientCamFocus);
                cameraFocusLeft = false;
                manager.GetComponent<NetworkView>().RPC("FocusCamera", RPCMode.Others, cameraFocusLeft);
            }
            else if (!cameraFocusLeft && manager.playerCamera.transform.position.x < moveLeftPos)
            {
                manager.cameraMan.FollowWaypoint(manager.serverCamFocus);
                cameraFocusLeft = true;
                manager.GetComponent<NetworkView>().RPC("FocusCamera", RPCMode.Others, cameraFocusLeft);
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
                    CannonClickListener cannonListener = hit2d.collider.GetComponent<CannonClickListener>();

                    if (cannonListener)
                    {
                        if (cannonListener.cannon == yourCannon && cannonListener.cannon.CanApplyBuff(selectedSpirit.type))
                        {
                            selectedSpirit.MoveHereAndTrigger(cannonListener.cannon.team == 0);

                            StartCoroutine(UseSpirit());

                            holdingSpirit = false;
                            selectedSpirit = null;

                            return;
                        }
                    }
                }

                if (yourTileSet.IsInside(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition)))
                {
                    selectedSpirit.healAOE.SetActive(false);
                    selectedSpirit.MoveHereAndHeal(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition), Network.isServer);

                    StartCoroutine(UseSpirit());

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

    IEnumerator UseSpirit()
    {
        spiritUsed = true;
        yield return new WaitForSeconds(0.8f);
        showButtons = false;
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

        yourCannon.GetComponent<Collider2D>().enabled = true;
    }
}
