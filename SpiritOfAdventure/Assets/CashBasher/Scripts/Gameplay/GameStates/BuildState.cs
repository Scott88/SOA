using UnityEngine;
using System.Collections;

public class BuildState : GameState
{
    private CashBasherManager manager;

    private BlockInventory selectedInventory;

    private int myTeam;

    private TileSet tileSet;

    private Vector3 lastTouchPos;

    private bool cameraFocusLeft;
    private bool cameraGrabbed;

    private float buildTimer;

    public BuildState(CashBasherManager gameManager, int team, TileSet yourSet, float timer)
    {
        manager = gameManager;
        myTeam = team;
        tileSet = yourSet;

        cameraFocusLeft = myTeam == 0;

        buildTimer = timer;
    }

    public void Prepare()
    {

    }

    public void Update()
    {
        if (buildTimer > 0f)
        {
            buildTimer -= Time.deltaTime;

            manager.gameText.text = "Time remaining: " + buildTimer.ToString("0");
        }
        else
        {
            if (Network.isClient)
            {
                manager.GetComponent<NetworkView>().RPC("OpponentReady", RPCMode.Server);
            }

            //manager.SwitchToState((int)GamePhase.GP_WAITING);
        }

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

    public void OnGUI()
    {

    }

    public void GetClickedOn()
    {
        Ray clickRay = manager.guiCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(clickRay, out hit))
        {
            BlockInventory inventory = hit.collider.gameObject.GetComponent<BlockInventory>();

            if (inventory)
            {
                if (inventory.IsSelected())
                {
                    DeselectInventory(inventory);
                }
                else
                {
                    SelectInventory(inventory);
                }

                cameraGrabbed = false;

                return;
            }
        }

        cameraGrabbed = true;
        lastTouchPos = manager.playerCamera.ScreenToWorldPoint(Input.mousePosition);
        manager.cameraMan.enabled = false;
    }

    public void GetHeldOn()
    {
        if (selectedInventory)
        {
            Vector3 position = manager.playerCamera.ScreenToWorldPoint(Input.mousePosition);

            if (tileSet.CanPlace(position))
            {
                selectedInventory.spawnIndicator.SetActive(true);
            }
            else
            {
                selectedInventory.spawnIndicator.SetActive(false);
            }

            position.x = Mathf.Floor(position.x) + 0.5f;
            position.y = Mathf.Floor(position.y) + 0.5f;
            position.z = 0;

            selectedInventory.spawnIndicator.transform.position = position;
        }
        else if(cameraGrabbed)
        {
            Vector3 nextPos = manager.playerCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector3 translation = new Vector3(lastTouchPos.x - nextPos.x, 0f);

            if (cameraFocusLeft && translation.x + manager.playerCamera.transform.position.x < -10f)
            {
                translation.x = -10f - manager.playerCamera.transform.position.x;
            }
            else if (!cameraFocusLeft && translation.x + manager.playerCamera.transform.position.x > 10f)
            {
                translation.x = 10f - manager.playerCamera.transform.position.x;
            }

            manager.playerCamera.transform.Translate(translation);
        }
    }

    public void GetReleasedOn()
    {
        if (selectedInventory)
        {
            selectedInventory.spawnIndicator.SetActive(false);

            Ray clickRay = manager.playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(clickRay, out hit))
            {
                Breakable breakable = hit.collider.gameObject.GetComponent<Breakable>();

                if (breakable)
                {
                    return;
                }
            }

            Vector3 position = manager.playerCamera.ScreenToWorldPoint(Input.mousePosition);

            if (tileSet.CanPlace(position))
            {
                PlaceBlock(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition), selectedInventory);
            }
        }
        else if (cameraGrabbed)
        {
            manager.cameraMan.enabled = true;

            if (cameraFocusLeft && manager.playerCamera.transform.position.x > -5f)
            {
                manager.cameraMan.FollowPosition(new Vector3(10f, 0f, 0f));
                cameraFocusLeft = false;
            }
            else if (!cameraFocusLeft && manager.playerCamera.transform.position.x < 5f)
            {
                manager.cameraMan.FollowPosition(new Vector3(-10f, 0f, 0f));
                cameraFocusLeft = true;
            }
        }
    }

    public void End()
    {
        if (selectedInventory)
        {
            selectedInventory.spawnIndicator.SetActive(false);

            DeselectInventory(selectedInventory);
        }
    }

    void SelectInventory(BlockInventory inventory)
    {
        selectedInventory = inventory;
        inventory.Select();
    }

    void PlaceBlock(Vector3 position, BlockInventory inventory)
    {
        position.x = Mathf.Floor(position.x) + 0.5f;
        position.y = Mathf.Floor(position.y) + 0.5f;
        position.z = 0;

        GameObject block = Network.Instantiate(inventory.GetBlock(), position, new Quaternion(), 0) as GameObject;

        inventory.Deselect();
        selectedInventory = null;
    }

    void DeselectInventory(BlockInventory inventory)
    {
        inventory.Deselect();
        selectedInventory = null;
    }

}
