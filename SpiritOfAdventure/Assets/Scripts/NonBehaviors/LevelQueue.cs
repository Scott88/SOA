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

    private static bool ads;

	public static void LoadLevel(int nextLevel, bool immediate = false, bool showAds = true)
	{
        previousLevel = Application.loadedLevel;

        if (immediate)
        {
            Application.LoadLevel(nextLevel);
        }
        else
        {
            ads = showAds;
            levelToLoadInt = nextLevel;
            usesInt = true;
            Application.LoadLevel("LoadingScreen");
        }
	}

    public static void LoadLevel(string nextLevel, bool immediate = false, bool showAds = true)
	{
        previousLevel = Application.loadedLevel;

        if (immediate)
        {
            Application.LoadLevel(nextLevel);
        }
        else
        {
            ads = showAds;
            levelToLoadString = nextLevel;
            usesInt = false;
            Application.LoadLevel("LoadingScreen");
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

    public static bool ShowAds()
    {
        return ads;
    }

    public static void LoadPreviousLevel()
    {
        Application.LoadLevel(previousLevel);
    }
}

