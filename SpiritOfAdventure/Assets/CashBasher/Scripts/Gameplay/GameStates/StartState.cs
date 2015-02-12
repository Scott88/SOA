using UnityEngine;
using System.Collections;

public class StartState : GameState
{
    public CashBasherManager manager;

    public float startDelay;

    private NetworkedLevelLoader loader;

    void Start()
    {
        loader = FindObjectOfType<NetworkedLevelLoader>();
    }

    public override void Prepare()
    {
        manager.gameText.text = "Get ready to start...!";
    }

    public override void UpdateState()
    {
        startDelay -= Time.deltaTime;

        if (startDelay <= 0f)
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
}
