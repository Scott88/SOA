using UnityEngine;
using System.Collections;

public class CashBasherManager : MonoBehaviour
{
    public Camera playerCamera, guiCamera;

    public CameraMan cameraMan;

    public TileSet serverSet, clientSet;

    public GameObject spawnIndicator;

    private int myTeam;

    private ArrayList blocks;

    private BlockInventory selectedInventory;

    void Start()
    {
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
    }

    public void AddBlock(Breakable b)
    {
        blocks.Add(b);
    }

    void Update()
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

    void GetClickedOn()
    {
        Ray clickRay = guiCamera.ScreenPointToRay(Input.mousePosition);
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

                return;
            }
        }
    }

    void GetHeldOn()
    {
        if (selectedInventory)
        {
            Vector3 position = playerCamera.ScreenToWorldPoint(Input.mousePosition);

            if (serverSet.CanPlace(position, myTeam) || clientSet.CanPlace(position, myTeam))
            {
                spawnIndicator.SetActive(true);
            }
            else
            {
                spawnIndicator.SetActive(false);
            }

            position.x = Mathf.Floor(position.x) + 0.5f;
            position.y = Mathf.Floor(position.y) + 0.5f;
            position.z = 0;

            spawnIndicator.transform.position = position;
        }
    }

    void GetReleasedOn()
    {
        if (selectedInventory)
        {
            spawnIndicator.SetActive(false);
        }

        Ray clickRay = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(clickRay, out hit))
        {
            Breakable breakable = hit.collider.gameObject.GetComponent<Breakable>();

            if (breakable)
            {
                //if (!playerInventory.IsSelected() && myTurn)
                //{
                //    TryBreakBlock(breakable);
                //}

                return;
            }

            //WinButton win = hit.collider.gameObject.GetComponent<WinButton>();

            //if (win)
            //{
            //    if (!playerInventory.IsSelected() && myTurn)
            //    {
            //        networkView.RPC("WinScreen", RPCMode.All, win.GetTeam());
            //    }

            //    return;
            //}
        }

        if (selectedInventory)
        {
            Vector3 position = playerCamera.ScreenToWorldPoint(Input.mousePosition);

            if (serverSet.CanPlace(position, myTeam) || clientSet.CanPlace(position, myTeam))
            {
                PlaceBlock(playerCamera.ScreenToWorldPoint(Input.mousePosition), selectedInventory);
            }
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

        Breakable b = block.GetComponent<Breakable>();
        b.SetTeam(myTeam);

        inventory.Deselect(true);
        selectedInventory = null;
    }

    void DeselectInventory(BlockInventory inventory)
    {
        inventory.Deselect(false);
        selectedInventory = null;
    }

    
}
