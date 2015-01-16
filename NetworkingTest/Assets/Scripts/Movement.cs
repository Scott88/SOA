using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

    public float speed = 1f;

    private bool myTurn = false;

    void Start()
    {
        if (networkView.isMine)
        {
            PlayerManager.SetPlayer(this);
        }
        else
        {
            PlayerManager.SetEnemy(this);
        }
    }

    void OnGUI()
    {
        if (Network.isClient || Network.isServer)
        {
            if (networkView.isMine && myTurn)
            {
                if (GUI.Button(new Rect(Screen.width * (0.1f), Screen.height * (0.805f), Screen.width * (0.07f), Screen.height * (0.07f)),
                               "^"))
                {
                    transform.Translate(0.0f, 1.0f, 0.0f);
                    myTurn = false;
                    PlayerManager.TheirTurn();
                }

                if (GUI.Button(new Rect(Screen.width * (0.025f), Screen.height * (0.855f), Screen.width * (0.07f), Screen.height * (0.07f)),
                               "<"))
                {
                    transform.Translate(-1.0f, 0.0f, 0.0f);
                    myTurn = false;
                    PlayerManager.TheirTurn();
                }

                if (GUI.Button(new Rect(Screen.width * (0.175f), Screen.height * (0.855f), Screen.width * (0.07f), Screen.height * (0.07f)),
                               ">"))
                {
                    transform.Translate(1.0f, 0.0f, 0.0f);
                    myTurn = false;
                    PlayerManager.TheirTurn();
                }

                if (GUI.Button(new Rect(Screen.width * (0.1f), Screen.height * (0.895f), Screen.width * (0.07f), Screen.height * (0.07f)),
                               "v"))
                {
                    transform.Translate(0.0f, -1.0f, 0.0f);
                    myTurn = false;
                    PlayerManager.TheirTurn();
                }
            }
        }
        else
        {
            if (GUI.Button(new Rect(Screen.width * (0.1f), Screen.height * (0.805f), Screen.width * (0.07f), Screen.height * (0.07f)),
                               "^"))
            {
                transform.Translate(0.0f, 1.0f, 0.0f);
            }

            if (GUI.Button(new Rect(Screen.width * (0.025f), Screen.height * (0.855f), Screen.width * (0.07f), Screen.height * (0.07f)),
                           "<"))
            {
                transform.Translate(-1.0f, 0.0f, 0.0f);
            }

            if (GUI.Button(new Rect(Screen.width * (0.175f), Screen.height * (0.855f), Screen.width * (0.07f), Screen.height * (0.07f)),
                           ">"))
            {
                transform.Translate(1.0f, 0.0f, 0.0f);
            }

            if (GUI.Button(new Rect(Screen.width * (0.1f), Screen.height * (0.895f), Screen.width * (0.07f), Screen.height * (0.07f)),
                           "v"))
            {
                transform.Translate(0.0f, -1.0f, 0.0f);
            }
        }
    }

    
    public void MyTurn()
    {
        networkView.RPC("SetMyTurn", RPCMode.All);
    }

    [RPC]
    void SetMyTurn()
    {
        myTurn = true;
    }
}
