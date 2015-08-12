using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class NetworkedLevelLoader : MonoBehaviour {

    public int othersCastleWorth { get; set; }
    public int othersCostume { get; set; }

    private static bool created = false;

    private int levelPrefix = 0;

    bool loaded, otherLoaded;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this);
            GetComponent<NetworkView>().group = 1;

            created = true;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public void LoadLevel(string levelName)
    {
        GetComponent<NetworkView>().RPC("LoadNetworkedLevel", RPCMode.AllBuffered, ++levelPrefix, levelName);
    }

    [RPC]
    void LoadNetworkedLevel(int prefix, string levelName)
    {
        levelPrefix = prefix;
        Network.SetSendingEnabled(0, false);
        Network.isMessageQueueRunning = false;

        loaded = false;
        otherLoaded = false;

        Network.SetLevelPrefix(prefix);
        LevelQueue.LoadLevel(levelName, false, false);
    }

    public void Ready(int enemyCostume)
    {
        Network.isMessageQueueRunning = true;
        Network.SetSendingEnabled(0, true);

        loaded = true;

        GetComponent<NetworkView>().RPC("OtherReady", RPCMode.Others, enemyCostume);
    }

    [RPC]
    void OtherReady(int enemyCostume)
    {
        otherLoaded = true;
        othersCostume = enemyCostume;

        GetComponent<NetworkView>().RPC("ConfirmOtherReady", RPCMode.Others, SaveFile.Instance().GetCurrentCostume());
    }

    [RPC]
    void ConfirmOtherReady(int enemyCostume)
    {
        otherLoaded = true;
        othersCostume = enemyCostume;
    }

    [RPC]
    void ReceiveOtherCastleWorth(int castleWorth)
    {
        othersCastleWorth = castleWorth;
    }

    public bool IsReady()
    {
        return loaded && otherLoaded;
    }
}
