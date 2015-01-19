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
        networkView.RPC("LoadNetworkedLevel", RPCMode.AllBuffered);
    }

    [RPC]
    void LoadNetworkedLevel()
    {
        Network.SetSendingEnabled(0, false);
        Network.isMessageQueueRunning = false;

        Network.SetLevelPrefix(++levelPrefix);
        Application.LoadLevel("game");

        Network.isMessageQueueRunning = true;
        Network.SetSendingEnabled(0, true);
    }
}
