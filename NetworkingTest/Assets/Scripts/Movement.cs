using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

    public float speed = 1f;

    bool myTurn;
	
	// Update is called once per frame
	void Update ()
    {
        if (Network.isClient || Network.isServer)
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
        }
	}

    [RPC]
    void TurnOver()
    {
        myTurn = true;
    }
}
