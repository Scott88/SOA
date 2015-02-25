using UnityEngine;
using System.Collections;

public class LevelQueue
{
	public static string menuContext = "MainMenu";
	public static bool startZoomed = false;

	private static int levelToLoadInt;
	private static string levelToLoadString;

    private static int previousLevel;

	private static bool usesInt;

	public static void LoadLevel(int nextLevel, bool immediate = false)
	{
        previousLevel = Application.loadedLevel;

        if (immediate)
        {
            Application.LoadLevel(nextLevel);
        }
        else
        {
            levelToLoadInt = nextLevel;
            usesInt = true;
            Application.LoadLevel(2);
        }
	}

    public static void LoadLevel(string nextLevel, bool immediate = false)
	{
        previousLevel = Application.loadedLevel;

        if (immediate)
        {
            Application.LoadLevel(nextLevel);
        }
        else
        {
            levelToLoadString = nextLevel;
            usesInt = false;
            Application.LoadLevel(2);
        }
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

    public static void LoadPreviousLevel()
    {
        Application.LoadLevel(previousLevel);
    }
}

