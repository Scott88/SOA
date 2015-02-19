using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class NetworkedLevelLoader : MonoBehaviour {

    public int othersCostume { get; set; }

    private static bool created = false;

    private int levelPrefix = 0;

    bool loaded, otherLoaded;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this);
            networkView.group = 1;

            created = true;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public void LoadLevel(string levelName)
    {
        networkView.RPC("LoadNetworkedLevel", RPCMode.AllBuffered, ++levelPrefix, levelName);
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
        LevelQueue.LoadLevel(levelName);
    }

    public void Ready(int enemyCostume)
    {
        Network.isMessageQueueRunning = true;
        Network.SetSendingEnabled(0, true);

        loaded = true;

        networkView.RPC("OtherReady", RPCMode.Others, enemyCostume);
    }

    [RPC]
    void OtherReady(int enemyCostume)
    {
        otherLoaded = true;
        othersCostume = enemyCostume;
    }

    [RPC]
    void ConfirmOtherReady(int enemyCostume)
    {

    }

    public bool IsReady()
    {
        return loaded && otherLoaded;
    }
}
