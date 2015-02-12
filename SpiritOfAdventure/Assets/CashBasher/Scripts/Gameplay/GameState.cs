using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour
{
    public virtual void Prepare() { }

    public virtual void UpdateState() { }

    public virtual void OnStateGUI() { }

    public virtual void End() { }
}
