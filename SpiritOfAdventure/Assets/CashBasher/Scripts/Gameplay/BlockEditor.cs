using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockEditor : MonoBehaviour
{
    public EditorTileSet tileSet;

    public Camera mainCamera;

    public int woodWorth = 5, stoneWorth = 10, metalWorth = 15, spiritWorth = 25;

    public BlockInventory woodInventory, stoneInventory, metalInventory;
    public BlockCounter woodCounter, stoneCounter, metalCounter;

    public AudioSource placeWoodSound, placeStoneSound, placeMetalSound;

    public SpiritInventory greenInventory, blueInventory, redInventory;
    public SpiritCounter greenCounter, blueCounter, redCounter;

    public GameObject greenTransfer, blueTransfer, redTransfer;

    private BlockInventory selectedInventory;

    private int castleWorth = 0;

    private bool blockBroken = false;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;

        IEnumerator<SaveFile.TileInfo> tileList = SaveFile.Instance().GetTileList();

        while (tileList.MoveNext())
        {
            switch (tileList.Current.type)
            {
                case BlockType.BT_WOOD:
                    if(TryLoad(tileList.Current.x, tileList.Current.y, woodInventory, woodCounter)) castleWorth += woodWorth;
                    break;
                case BlockType.BT_STONE:
                    if(TryLoad(tileList.Current.x, tileList.Current.y, stoneInventory, stoneCounter)) castleWorth += stoneWorth;
                    break;
                case BlockType.BT_METAL:
                    if(TryLoad(tileList.Current.x, tileList.Current.y, metalInventory, metalCounter)) castleWorth += metalWorth;
                    break;
            }
        }

        TryTransfer(SaveFile.Instance().GetSpiritCount(SpiritType.ST_GREEN), greenInventory, greenCounter);
        TryTransfer(SaveFile.Instance().GetSpiritCount(SpiritType.ST_BLUE), blueInventory, blueCounter);
        TryTransfer(SaveFile.Instance().GetSpiritCount(SpiritType.ST_RED), redInventory, redCounter);
    }

    bool TryLoad(int x, int y, BlockInventory inventory, BlockCounter counter)
    {
        if (!inventory.Empty())
        {
            inventory.TakeBlock();

            tileSet.LoadBlock(x, y, inventory.GetBlock());

            counter.Add();

            return true;
        }

        return false;
    }

    void TryTransfer(int count, SpiritInventory inventory, SpiritCounter counter)
    {
        castleWorth += spiritWorth * count;
        counter.Add(inventory.RemoveSpirits(count));
    }

    public int GetCastleWorth() { return castleWorth; }
    
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
                selectedInventory.spawnIndicator.transform.position = tileSet.CenterOn(position);
            }
            else
            {
                selectedInventory.spawnIndicator.SetActive(false);
            }
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
        if (inventory.selectable)
        {
            if (selectedInventory)
            {
                DeselectInventory(selectedInventory);
            }

            selectedInventory = inventory;
            inventory.Select();
        }
    }

    void DeselectInventory(BlockInventory inventory)
    {
        if (inventory.selectable)
        {
            inventory.Deselect(false);
            selectedInventory = null;
        }
    }

    void PlaceBlock(Vector3 position, BlockInventory inventory)
    {
        BlockType type = tileSet.PlaceAndSaveBlock(position, inventory.GetBlock());

        switch (type)
        {
            case BlockType.BT_WOOD:
                woodCounter.Add();
                castleWorth += woodWorth;
                if (placeWoodSound) placeWoodSound.Play();
                break;
            case BlockType.BT_STONE:
                stoneCounter.Add();
                castleWorth += stoneWorth;
                if (placeStoneSound) placeStoneSound.Play();
                break;
            case BlockType.BT_METAL:
                metalCounter.Add();
                castleWorth += metalWorth;
                if (placeMetalSound) placeMetalSound.Play();
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
                castleWorth -= woodWorth;
                woodCounter.Remove();
                break;
            case BlockType.BT_STONE:
                stoneInventory.ReturnBlock();
                castleWorth -= stoneWorth;
                stoneCounter.Remove();
                break;
            case BlockType.BT_METAL:
                metalInventory.ReturnBlock();
                castleWorth -= metalWorth;
                metalCounter.Remove();
                break;
        }

        tileSet.RemoveBlock(block.gameObject);
        Destroy(block.gameObject);
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

        castleWorth += spiritWorth;
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

        castleWorth -= spiritWorth;
    }

    void RemoveSpiritFromPool(SpiritInventory inventory, SpiritCounter counter, GameObject transfer)
    {
        inventory.AddSpirit();
        counter.Remove();

        GameObject t = Instantiate(transfer, counter.transform.position, new Quaternion()) as GameObject;

        SmoothDamper damper = t.GetComponent<SmoothDamper>();
        damper.target = inventory.transform.position;
    }

#if UNITY_EDITOR
    void OnApplicationQuit()
    {
        SaveFile.Instance().SaveToXML();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveFile.Instance().SaveToXML();
        }
    }
#endif
}
