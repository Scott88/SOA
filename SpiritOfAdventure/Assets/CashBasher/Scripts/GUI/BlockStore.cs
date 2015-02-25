using UnityEngine;
using System.Collections;

public class BlockStore : MonoBehaviour
{
    public int woodPrice = 5, stonePrice = 10, metalPrice = 15;

    public StarInventory starInventory;

    public BlockInventory woodInventory, stoneInventory, metalInventory;

    public Camera mainCamera;

    public GameObject menu;

    public GUISkin mySkin;

    void OnGUI()
    {
        GUI.skin = mySkin;

        if (GUI.Button(new Rect(Screen.width * (0.85f), Screen.height * (0.05f), Screen.width * (0.12f), Screen.height * (0.1f)),
                               "Back"))
        {
            mainCamera.transform.position = new Vector3(0f, 0f, -10f);

            gameObject.SetActive(false);
            menu.SetActive(true);
        }

        if (GUI.Button(new Rect(Screen.width * (0.05f), Screen.height * (0.05f), Screen.width * (0.12f), Screen.height * (0.1f)),
                               "Buy Blocks"))
        {
            LevelQueue.LoadLevel("HorizontalTabs", true);
        }

        if (starInventory.GetStars() >= woodPrice)
        {
            if (GUI.Button(new Rect(Screen.width * (0.05f), Screen.height * (0.4f), Screen.width * (0.25f), Screen.height * (0.2f)),
                                   "BUY WOOD:\n" + woodPrice + " STARS"))
            {
                woodInventory.ReturnBlock();
                starInventory.Remove(woodPrice);
            }
        }

        if (starInventory.GetStars() >= stonePrice)
        {
            if (GUI.Button(new Rect(Screen.width * (0.375f), Screen.height * (0.4f), Screen.width * (0.25f), Screen.height * (0.2f)),
                                   "BUY STONE:\n" + stonePrice + " STARS"))
            {
                stoneInventory.ReturnBlock();
                starInventory.Remove(stonePrice);
            }
        }

        if (starInventory.GetStars() >= metalPrice)
        {
            if (GUI.Button(new Rect(Screen.width * (0.70f), Screen.height * (0.4f), Screen.width * (0.25f), Screen.height * (0.2f)),
                                   "BUY METAL:\n" + metalPrice + " STARS"))
            {
                metalInventory.ReturnBlock();
                starInventory.Remove(metalPrice);
            }
        }
    }
}
