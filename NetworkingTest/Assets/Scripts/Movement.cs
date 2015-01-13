using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

    public float speed = 1f;

    public bool myTurn = true;

    //void Start()
    //{
    //    if (Network.isServer)
    //    {
    //        PlayerSpawner spawner = FindObjectOfType<PlayerSpawner>();

    //        spawner.AddPlayer(this);
    //    }
    //}
	
	// Update is called once per frame
	void Update ()
    {
        /*if (Network.isClient || Network.isServer)
        {
            if (networkView.isMine && myTurn)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    transform.Translate(0.0f, 1.0f, 0.0f);
                    myTurn = false;
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    transform.Translate(0.0f, -1.0f, 0.0f);
                    myTurn = false;
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    transform.Translate(-1.0f, 0.0f, 0.0f);
                    myTurn = false;
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    transform.Translate(1.0f, 0.0f, 0.0f);
                    myTurn = false;
                }

                if (!myTurn)
                {
                    networkView.RPC("TurnOver", RPCMode.Others);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                transform.Translate(0.0f, 1.0f, 0.0f);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                transform.Translate(0.0f, -1.0f, 0.0f);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                transform.Translate(-1.0f, 0.0f, 0.0f);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                transform.Translate(1.0f, 0.0f, 0.0f);
            }
        }*/
	}

    void OnGUI()
    {
        if (Network.isClient || Network.isServer)
        {
            if (networkView.isMine && myTurn)
            {
                if (GUI.Button(new Rect(Screen.width * (0.075f), Screen.height * (0.825f), Screen.width * (0.05f), Screen.height * (0.05f)),
                               "^"))
                {
                    transform.Translate(0.0f, 1.0f, 0.0f);
                    myTurn = false;
                }

                if (GUI.Button(new Rect(Screen.width * (0.025f), Screen.height * (0.875f), Screen.width * (0.05f), Screen.height * (0.05f)),
                               "<"))
                {
                    transform.Translate(-1.0f, 0.0f, 0.0f);
                    myTurn = false;
                }

                if (GUI.Button(new Rect(Screen.width * (0.125f), Screen.height * (0.875f), Screen.width * (0.05f), Screen.height * (0.05f)),
                               ">"))
                {
                    transform.Translate(1.0f, 0.0f, 0.0f);
                    myTurn = false;
                }

                if (GUI.Button(new Rect(Screen.width * (0.075f), Screen.height * (0.925f), Screen.width * (0.05f), Screen.height * (0.05f)),
                               "v"))
                {
                    transform.Translate(0.0f, -1.0f, 0.0f);
                    myTurn = false;
                }

                if (!myTurn)
                {
                    networkView.RPC("TurnOver", RPCMode.Others);
					Debug.Log ("sending Turnover RPC!");
                }
            }
        }
        else
        {
            if (GUI.Button(new Rect(Screen.width * (0.075f), Screen.height * (0.825f), Screen.width * (0.05f), Screen.height * (0.05f)),
                               "^"))
            {
                transform.Translate(0.0f, 1.0f, 0.0f);
            }

            if (GUI.Button(new Rect(Screen.width * (0.025f), Screen.height * (0.875f), Screen.width * (0.05f), Screen.height * (0.05f)),
                           "<"))
            {
                transform.Translate(-1.0f, 0.0f, 0.0f);
            }

            if (GUI.Button(new Rect(Screen.width * (0.125f), Screen.height * (0.875f), Screen.width * (0.05f), Screen.height * (0.05f)),
                           ">"))
            {
                transform.Translate(1.0f, 0.0f, 0.0f);
            }

            if (GUI.Button(new Rect(Screen.width * (0.075f), Screen.height * (0.925f), Screen.width * (0.05f), Screen.height * (0.05f)),
                           "v"))
            {
                transform.Translate(0.0f, -1.0f, 0.0f);
            }
        }
    }

    [RPC]
    void TurnOver()
    {
        myTurn = true;
		Debug.Log ("TURN");
    }
}
