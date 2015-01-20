using UnityEngine;
using System.Collections;

public class WinButton : MonoBehaviour {

    private int team = -1;

    public void SetTeam(int t)
    {
        networkView.RPC("NetSetTeam", RPCMode.All, t);
    }

    [RPC]
    void NetSetTeam(int t)
    {
        team = t;
    }

    public int GetTeam()
    {
        return team;
    }
}
