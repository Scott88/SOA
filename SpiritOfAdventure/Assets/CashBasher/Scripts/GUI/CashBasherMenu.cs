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

        if (GUI.Button(new Rect(Screen.width * (0.85f), Screen.height * (0.05f), Screen.width * (0.12f), Screen.height * (0.1f)),
                               "Back"))
        {
            Application.LoadLevel("MainMenu");
        }

        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            if (GUI.Button(new Rect(Screen.width * (0.85f), Screen.height * (0.7f), Screen.width * (0.12f), Screen.height * (0.1f)),
                           "Start Your Own Game"))
            {
                gameObject.SetActive(false);
                serverMenu.gameObject.SetActive(true);
                serverMenu.CreateServer();
            }

            if (GUI.Button(new Rect(Screen.width * (0.85f), Screen.height * (0.85f), Screen.width * (0.12f), Screen.height * (0.1f)),
                           "Look For A Game"))
            {
                gameObject.SetActive(false);
                clientMenu.gameObject.SetActive(true);
                clientMenu.SearchForServers();
            }
        }
    }
}
