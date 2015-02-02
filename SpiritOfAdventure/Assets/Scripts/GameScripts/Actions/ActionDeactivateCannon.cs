using UnityEngine;
using System.Collections;

public class ActionDeactivateCannon : SOAAction
{
    public Cannon cannon;
    GameManager manager;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
    }

    protected override void Activate()
    {
        cannon.Deactivate();
        manager.SetCannonActive(false);
    }
}