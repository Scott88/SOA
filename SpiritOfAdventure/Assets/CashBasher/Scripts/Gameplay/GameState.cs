using UnityEngine;
using System.Collections;

public interface GameState
{
    void Prepare();

    void GetClickedOn();
    void GetHeldOn();
    void GetReleasedOn();

    void End();
}
