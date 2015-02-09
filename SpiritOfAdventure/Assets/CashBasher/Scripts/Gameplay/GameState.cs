using UnityEngine;
using System.Collections;

public interface GameState
{
    void Prepare();

    void Update();

    void OnGUI();

    void End();
}
