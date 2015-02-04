using UnityEngine;
using System.Collections;

public class BlockEditor : MonoBehaviour
{
    public TileSet tileSet;

    public Camera mainCamera;

    public BlockInventory woodInventory, stoneInventory, metalInventory;
    public BlockCounter woodCounter, stoneCounter, metalCounter;

    public SpiritInventory greenInventory, blueInventory, redInventory;
    public SpiritCounter greenCounter, blueCounter, redCounter;

    public GameObject greenTransfer, blueTransfer, redTransfer;

    private BlockInventory selectedInventory;

    private bool blockBroken = false;
    
    // Update is called once per frame
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

    public void GetClickedOn()
    {
        blockBroken = false;

        Ray clickRay = mainCamera.ScreenPointToRay(Input.mousePosition);
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
                else if (!inventory.Empty())
                {
                    SelectInventory(inventory);
                }

                return;
            }
        }

        //We start with the spiritGUI as it has the highest priority so we raycast from the spiritGUI camera to the
        // GUI objects.
        Vector2 rayOrigin = (Vector2)(mainCamera.ScreenToWorldPoint(Input.mousePosition));
        Vector2 rayDirection = new Vector2();

        RaycastHit2D hit2d = Physics2D.Raycast(rayOrigin, rayDirection);

        if (hit2d)
        {
            Breakable breakable = hit2d.collider.gameObject.GetComponent<Breakable>();

            if (breakable)
            {
                RemoveBlock(breakable);
                blockBroken = true;
                return;
            }

            SpiritInventory inventory = hit2d.collider.gameObject.GetComponent<SpiritInventory>();

            if (inventory)
            {
                if (!inventory.Empty())
                {
                    AddSpiritToPool(inventory.type);
                    return;
                }
            }

            SpiritCounter counter = hit2d.collider.gameObject.GetComponent<SpiritCounter>();

            if (counter)
            {
                if (!counter.Empty())
                {
                    RemoveSpiritFromPool(counter.type);
                    return;
                }
            }
        }
    }

    public void GetHeldOn()
    {
        if (selectedInventory && !blockBroken)
        {
            Vector3 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            //We start with the spiritGUI as it has the highest priority so we raycast from the spiritGUI camera to the
            // GUI objects.
            Vector2 rayOrigin = (Vector2)(mainCamera.ScreenToWorldPoint(Input.mousePosition));
            Vector2 rayDirection = new Vector2();

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection);

            Breakable breakable = null;

            if (hit)
            {
                breakable = hit.collider.gameObject.GetComponent<Breakable>();
            }

            if (tileSet.CanPlace(position) && breakable == null)
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
    }

    public void GetReleasedOn()
    {
        if (selectedInventory && !blockBroken)
        {
            selectedInventory.spawnIndicator.SetActive(false);

            //We start with the spiritGUI as it has the highest priority so we raycast from the spiritGUI camera to the
            // GUI objects.
            Vector2 rayOrigin = (Vector2)(mainCamera.ScreenToWorldPoint(Input.mousePosition));
            Vector2 rayDirection = new Vector2();

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, 100);

            if (hit)
            {
                Breakable breakable = hit.collider.gameObject.GetComponent<Breakable>();

                if (breakable)
                {
                    return;
                }
            }

            Vector3 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            if (tileSet.CanPlace(position))
            {
                PlaceBlock(mainCamera.ScreenToWorldPoint(Input.mousePosition), selectedInventory);
            }
        }
    }

    void SelectInventory(BlockInventory inventory)
    {
        if (selectedInventory)
        {
            DeselectInventory(selectedInventory);
        }

        selectedInventory = inventory;
        inventory.Select();
    }

    void PlaceBlock(Vector3 position, BlockInventory inventory)
    {
        BlockType type = tileSet.PlaceBlock(position, inventory.GetBlock());

        switch (type)
        {
            case BlockType.BT_WOOD:
                woodCounter.Add();
                break;
            case BlockType.BT_STONE:
                stoneCounter.Add();
                break;
            case BlockType.BT_METAL:
                metalCounter.Add();
                break;
        }

        inventory.Deselect(true);
        selectedInventory = null;
    }

    void RemoveBlock(Breakable block)
    {
        switch (block.type)
        {
            case BlockType.BT_WOOD:
                woodInventory.ReturnBlock();
                woodCounter.Remove();
                break;
            case BlockType.BT_STONE:
                stoneInventory.ReturnBlock();
                stoneCounter.Remove();
                break;
            case BlockType.BT_METAL:
                metalInventory.ReturnBlock();
                metalCounter.Remove();
                break;
        }

        tileSet.RemoveBlock(block.gameObject);

        Destroy(block.gameObject);
    }

    void DeselectInventory(BlockInventory inventory)
    {
        inventory.Deselect(false);
        selectedInventory = null;
    }

    void AddSpiritToPool(SpiritType type)
    {
        switch (type)
        {
            case SpiritType.ST_GREEN:
                AddSpiritToPool(greenInventory, greenCounter, greenTransfer);
                break;
            case SpiritType.ST_BLUE:
                AddSpiritToPool(blueInventory, blueCounter, blueTransfer);
                break;
            case SpiritType.ST_RED:
                AddSpiritToPool(redInventory, redCounter, redTransfer);
                break;
        }
    }

    void AddSpiritToPool(SpiritInventory inventory, SpiritCounter counter, GameObject transfer)
    {
        inventory.RemoveSpirit();
        counter.Add();

        GameObject t = Instantiate(transfer, inventory.transform.position, new Quaternion()) as GameObject;

        SmoothDamper damper = t.GetComponent<SmoothDamper>();
        damper.target = counter.transform.position;
    }

    void RemoveSpiritFromPool(SpiritType type)
    {
        switch (type)
        {
            case SpiritType.ST_GREEN:
                RemoveSpiritFromPool(greenInventory, greenCounter, greenTransfer);
                break;
            case SpiritType.ST_BLUE:
                RemoveSpiritFromPool(blueInventory, blueCounter, blueTransfer);
                break;
            case SpiritType.ST_RED:
                RemoveSpiritFromPool(redInventory, redCounter, redTransfer);
                break;
        }
    }

    void RemoveSpiritFromPool(SpiritInventory inventory, SpiritCounter counter, GameObject transfer)
    {
        inventory.AddSpirit();
        counter.Remove();

        GameObject t = Instantiate(transfer, counter.transform.position, new Quaternion()) as GameObject;

        SmoothDamper damper = t.GetComponent<SmoothDamper>();
        damper.target = inventory.transform.position;
    }
}
