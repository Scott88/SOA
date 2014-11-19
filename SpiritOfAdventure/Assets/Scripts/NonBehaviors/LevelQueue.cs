using UnityEngine;
using System.Collections;

public class LevelQueue
{
	public static string menuContext = "MainMenu";
	public static bool startZoomed = false;

	private static int levelToLoadInt;
	private static string levelToLoadString;

	private static bool usesInt;

	public static void LoadLevel(int nextLevel)
	{
		levelToLoadInt = nextLevel;
		usesInt = true;
		Application.LoadLevel(2);
	}

	public static void LoadLevel(string nextLevel)
	{
		levelToLoadString = nextLevel;
		usesInt = false;
		Application.LoadLevel(2);
	}

	public static bool UsesInt()
	{
		return usesInt;
	}

	public static int GetNextLevel()
	{
		return levelToLoadInt;
	}

	public static string GetNextLevelString()
	{
		return levelToLoadString;
	}
}

