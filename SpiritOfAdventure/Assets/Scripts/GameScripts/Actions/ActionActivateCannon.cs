using UnityEngine;
using System.Collections;

public class ActionActivateCannon : SOAAction {

    public Cannon cannon;
    GameManager manager;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
    }

    protected override void Activate()
    {
        cannon.Activate();
        manager.SetCannonActive(true);
    }
}
