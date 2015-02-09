using UnityEngine;
using System.Collections;

public class StartState : GameState
{
    CashBasherManager manager;

    float delay;

    NetworkedLevelLoader loader;

    public StartState(CashBasherManager m, float startDelay, NetworkedLevelLoader load)
    {
        delay = startDelay;
        manager = m;
        loader = load;
    }

    public void Prepare()
    {
        manager.gameText.text = "Get ready to start...!";
    }

    public void Update()
    {
        delay -= Time.deltaTime;

        if (delay <= 0f)
        {
            if (Network.isServer && loader.IsReady())
            {
                manager.RandomizeTreasure();

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
    }

    public void OnGUI()
    {

    }

    public void End()
    {
        
    }
}
