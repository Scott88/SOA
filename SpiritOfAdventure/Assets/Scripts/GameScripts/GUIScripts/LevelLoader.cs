using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    public TextMesh hintDisplay;

	void Start()
	{
        hintDisplay.text = FindObjectOfType<HintList>().GetRandomHint();

        if (LevelQueue.ShowAds())
        {
            AdPlayer player = FindObjectOfType<AdPlayer>();

            if (player)
            {
                player.TryShowAd();
            }
            else
            {
                Load();
            }
        }
        else
        {
            Load();
        }
	}

    public void Load()
    {
        if (LevelQueue.UsesInt())
        {
            Application.LoadLevel(LevelQueue.GetNextLevel());
        }
        else
        {
            Application.LoadLevel(LevelQueue.GetNextLevelString());
        }
    }
}