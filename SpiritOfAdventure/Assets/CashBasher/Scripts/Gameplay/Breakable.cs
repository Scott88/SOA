using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class Breakable : MonoBehaviour {

    public bool isBreakable = true;

    private int team = -1;

    void Start()
    {
        FindObjectOfType<CashBasherManager>().AddBlock(this);
    }

    public bool Break()
    {
        if (isBreakable)
        {
            Destroy(gameObject);
        }

        return isBreakable;       
    }

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
