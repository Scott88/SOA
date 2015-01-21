using UnityEngine;
using System.Collections;

public class CashBasherMenu : MonoBehaviour
{
    public GUISkin mySkin;

    public ServerMenu serverMenu;
    public ClientMenu clientMenu;

    void OnGUI()
    {
        GUI.skin = mySkin;

        if (GUI.Button(new Rect(Screen.width * (0.375f), Screen.height * (0.1f), Screen.width * (0.25f), Screen.height * (0.166f)),
                               "Back"))
        {
            Application.LoadLevel("MiniGameMenu");
        }

        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            if (GUI.Button(new Rect(Screen.width * (0.125f), Screen.height * (0.4f), Screen.width * (1f / 4f), Screen.height * (1f / 6f)),
                           "Start Your Own Game"))
            {
                gameObject.SetActive(false);
                serverMenu.gameObject.SetActive(true);
                serverMenu.CreateServer();
            }

            if (GUI.Button(new Rect(Screen.width * (0.625f), Screen.height * (0.4f), Screen.width * (1f / 4f), Screen.height * (1f / 6f)),
                           "Look For A Game"))
            {
                gameObject.SetActive(false);
                clientMenu.gameObject.SetActive(true);
                clientMenu.SearchForServers();
            }
        }
    }
}
