using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour {

    public bool isBreakable = true;

    private int team = -1;

    void Start()
    {
        GameManager manager = FindObjectOfType<GameManager>();

        manager.AddBreakable(this);
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
