using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour {

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

    public void LoadLevel()
    {
        networkView.RPC("LoadNetworkedLevel", RPCMode.AllBuffered, ++levelPrefix);
    }

    [RPC]
    void LoadNetworkedLevel(int prefix)
    {
        levelPrefix = prefix;
        Network.SetSendingEnabled(0, false);
        Network.isMessageQueueRunning = false;

        Network.SetLevelPrefix(prefix);
        Application.LoadLevel("game");

        Network.isMessageQueueRunning = true;
        Network.SetSendingEnabled(0, true);
    }
}
