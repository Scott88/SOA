using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class NetworkedLevelLoader : MonoBehaviour {

    private static bool created = false;

    private int levelPrefix = 0;

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

        Network.SetLevelPrefix(prefix);
        LevelQueue.LoadLevel(levelName);
    }

    public void EnableSending()
    {
        Network.isMessageQueueRunning = true;
        Network.SetSendingEnabled(0, true);
    }
}
