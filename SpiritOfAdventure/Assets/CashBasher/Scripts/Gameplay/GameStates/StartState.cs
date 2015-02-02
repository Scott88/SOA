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
        
    }

    public void Update()
    {
        delay -= Time.deltaTime;

        if (delay <= 0f)
        {
            if (Network.isServer && loader.IsReady())
            {
                manager.RandomizeTreasure();

                manager.networkView.RPC("SwitchToState", RPCMode.All, (int)GamePhase.GP_BUILD);
            }
        }
    }

    public void End()
    {
        
    }
}
