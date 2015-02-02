using UnityEngine;
using System.Collections;

public class WaitingState : GameState 
{
    CashBasherManager manager;

    public WaitingState(CashBasherManager m)
    {
        manager = m;   
    }

    public void Prepare()
    {

    }

    public void Update()
    {
        if (Network.isServer && manager.opponentIsReady)
        {
            bool serverFirst = Random.value > 0.5f;

            if (serverFirst)
            {
                manager.networkView.RPC("SwitchToState", RPCMode.Others, (int)GamePhase.GP_THEIR_TURN);
                manager.SwitchToState((int)GamePhase.GP_YOUR_TURN);              
            }
            else
            {
                manager.networkView.RPC("SwitchToState", RPCMode.Others, (int)GamePhase.GP_YOUR_TURN);
                manager.SwitchToState((int)GamePhase.GP_THEIR_TURN);               
            }
        }
    }

    public void End()
    {

    }
}
