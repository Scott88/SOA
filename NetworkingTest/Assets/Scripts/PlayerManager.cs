using UnityEngine;
using System.Collections;

public class PlayerManager {

    private static Movement player, enemy;

    public static void SetPlayer(Movement p)
    {
        player = p;
    }

    public static void SetEnemy(Movement e)
    {
        enemy = e;
    }

    public static void TheirTurn()
    {
        enemy.MyTurn();
    }
}
