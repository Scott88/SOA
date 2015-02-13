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

        if (Network.isServer)
        {
            manager.StartCoroutine(Preshow());
        }
    }

    IEnumerator Preshow()
    {
        yield return new WaitForSeconds(startDelay);

        while (!loader.IsReady())
        {
            yield return null;
        }

        manager.RandomizeTreasure();

        manager.networkView.RPC("FadeSplashScreen", RPCMode.All);

        yield return new WaitForSeconds(1.0f);

        manager.networkView.RPC("FocusCamera", RPCMode.All, true);

        yield return new WaitForSeconds(1.5f);

        manager.GenerateServerSpirits();

        yield return new WaitForSeconds(1.0f);

        manager.networkView.RPC("FocusCamera", RPCMode.All, false);

        yield return new WaitForSeconds(1.5f);

        manager.GenerateClientSpirits();

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
