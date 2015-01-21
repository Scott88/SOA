using UnityEngine;
using System.Collections;

public class BuildState : GameState
{
    private CashBasherManager manager;

    private BlockInventory selectedInventory;

    private int myTeam;

    private TileSet tileSet;

    private GameObject spawnIndicator;

    public BuildState(CashBasherManager gameManager, int team, TileSet yourSet, GameObject indicator)
    {
        manager = gameManager;
        myTeam = team;
        tileSet = yourSet;
        spawnIndicator = indicator;
    }

    public void Prepare()
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

                return;
            }
        }
    }

    public void GetHeldOn()
    {
        if (selectedInventory)
        {
            Vector3 position = manager.playerCamera.ScreenToWorldPoint(Input.mousePosition);

            if (tileSet.CanPlace(position))
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

    public void GetReleasedOn()
    {
        if (selectedInventory)
        {
            spawnIndicator.SetActive(false);
        }

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

        if (selectedInventory)
        {
            Vector3 position = manager.playerCamera.ScreenToWorldPoint(Input.mousePosition);

            if (tileSet.CanPlace(position))
            {
                PlaceBlock(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition), selectedInventory);
            }
        }
    }

    public void End()
    {

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
