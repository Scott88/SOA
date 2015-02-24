using UnityEngine;
using System.Collections;

public class CashBasherMenu : MonoBehaviour
{
    public GUISkin mySkin;

    public ServerMenu serverMenu;
    public ClientMenu clientMenu;
    public BlockStore blockStore;

    public Camera mainCamera;

    void OnGUI()
    {
        GUI.skin = mySkin;

        if (GUI.Button(new Rect(Screen.width * (0.85f), Screen.height * (0.05f), Screen.width * (0.12f), Screen.height * (0.1f)),
                               "Back"))
        {
            Application.LoadLevel("MainMenu");
        }

        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            if (GUI.Button(new Rect(Screen.width * (0.85f), Screen.height * (0.2f), Screen.width * (0.12f), Screen.height * (0.1f)),
                           "Buy Blocks"))
            {
                mainCamera.transform.position = new Vector3(-30f, 0f, -10f);
                gameObject.SetActive(false);
                blockStore.gameObject.SetActive(true);
            }

            if (GUI.Button(new Rect(Screen.width * (0.85f), Screen.height * (0.7f), Screen.width * (0.12f), Screen.height * (0.1f)),
                           "Start Your Own Game"))
            {
                mainCamera.transform.position = new Vector3(20f, 0f, -10f);
                gameObject.SetActive(false);
                serverMenu.gameObject.SetActive(true);
                serverMenu.CreateServer();
            }

            if (GUI.Button(new Rect(Screen.width * (0.85f), Screen.height * (0.85f), Screen.width * (0.12f), Screen.height * (0.1f)),
                           "Look For A Game"))
            {
                mainCamera.transform.position = new Vector3(20f, 0f, -10f);
                gameObject.SetActive(false);
                clientMenu.gameObject.SetActive(true);
                clientMenu.SearchForServers();
            }
        }
    }
}
