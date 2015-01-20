using UnityEngine;
using System.Collections;

public class PlayerManager {

    private static Movement player, enemy;

    public static void SetPlayer(Movement p)
    {
        player = p;

        //if (enemy != null)
        //{
        //    WhoGoesFirst();
        //}
    }

    public static void SetEnemy(Movement e)
    {
        enemy = e;

        //if (player != null)
        //{
        //    WhoGoesFirst();
        //}
    }

    public static void WhoGoesFirst()
    {
        if (Network.isServer)
        {
			float randomValue = Random.value;

			if (randomValue > 0.5f)
            {
                TheirTurn();
            }
            else
            {
				TheirTurn();
			}
		}
    }

    public static void TheirTurn()
    {
        enemy.MyTurn();
    }

    public static void OurTurn()
    {
        player.MyTurn();
    }
}
