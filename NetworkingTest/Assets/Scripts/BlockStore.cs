using UnityEngine;
using System.Collections;

public class BlockStore : MonoBehaviour {

    public Networker networker;

    public TextMesh pointDisplay, blockDisplay;

    private bool openMenu = false;

    private int points, blocks;

    void Start()
    {
        if (PlayerPrefs.HasKey("Points"))
        {
            points = PlayerPrefs.GetInt("Points");
        }
        else
        {
            points = 15;
            PlayerPrefs.SetInt("Points", points);
        }

        if (PlayerPrefs.HasKey("Blocks"))
        {
            blocks = PlayerPrefs.GetInt("Blocks");
        }
        else
        {
            blocks = 4;
            PlayerPrefs.SetInt("Blocks", blocks);
        }

        pointDisplay.text = "Points: " + points.ToString();
        blockDisplay.text = "Blocks: " + blocks.ToString();
    }

    void OnGUI()
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            if (!openMenu)
            {
                if (GUI.Button(new Rect(Screen.width * (0.375f), Screen.height * (0.1f), Screen.width * (0.25f), Screen.height * (0.166f)),
                               "Block Store"))
                {
                    pointDisplay.gameObject.SetActive(true);
                    blockDisplay.gameObject.SetActive(true);

                    networker.showOptions = false;
                    openMenu = true;
                }
            }
            else
            {
                if (points >= 5)
                {
                    if (GUI.Button(new Rect(Screen.width * (0.125f), Screen.height * (0.4f), Screen.width * (0.25f), Screen.height * (0.166f)),
                                    "Buy a Block: 15 Points"))
                    {
                        points -= 5;
                        blocks += 1;

                        pointDisplay.text = "Points: " + points.ToString();
                        blockDisplay.text = "Blocks: " + blocks.ToString();
                    }
                }

                if (GUI.Button(new Rect(Screen.width * (0.375f), Screen.height * (0.1f), Screen.width * (0.25f), Screen.height * (0.166f)),
                               "Leave"))
                {
                    PlayerPrefs.SetInt("Points", points);
                    PlayerPrefs.SetInt("Blocks", blocks);

                    pointDisplay.gameObject.SetActive(false);
                    blockDisplay.gameObject.SetActive(false);

                    openMenu = false;
                    networker.showOptions = true;
                }
            }
        }
    }
}
