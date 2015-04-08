using UnityEngine;
using System.Collections;

using UnityEngine.Advertisements;


public class MainMenu : FocusPoint
{
	public GUISkin mySkin;
	public FocusPoint firstChapter, credits;

	private MenuFocus focus;

	void Start()
	{
        SaveFile.Instance();

		focus = FindObjectOfType<MenuFocus>() as MenuFocus;

		if (Screen.width >= (1080)) 
		{
			mySkin.button.fontSize = Screen.width / (40);
			mySkin.label.fontSize = Screen.width / (20);
		}

		if (Screen.width < (1080))
		{
			mySkin.button.fontSize = Screen.width / (30);
			mySkin.label.fontSize = Screen.width / (20);
		}

		if (gameObject.name != LevelQueue.menuContext)
		{
			gameObject.SetActive(false);
		}
	}

    void OnGUI()
    {
        GUI.skin = mySkin;

        GUI.Label(new Rect(Screen.width * (0.19f), Screen.height * (0.0f), Screen.width * (0.6f), Screen.height * (0.2f)), "Spirit Of Adventure");

        if (!AdReward.viewed)
        {
            if (GUI.Button(new Rect(Screen.width * (0.35f), Screen.height * (0.2f), Screen.width * (0.25f), Screen.height * (0.14f)), "Start"))
            {
                focus.PlayButtonSound();
                focus.SetTarget(firstChapter);
            }

            if (GUI.Button(new Rect(Screen.width * (0.35f), Screen.height * (0.35f), Screen.width * (0.25f), Screen.height * (0.14f)), "Versus"))
            {
                focus.PlayButtonSound();
                Application.LoadLevel("MiniGameMenu");
            }

            if (GUI.Button(new Rect(Screen.width * (0.325f), Screen.height * (0.50f), Screen.width * (0.3f), Screen.height * (0.14f)), "Free Lily Bucks"))
            {
                focus.PlayButtonSound();
                Application.LoadLevel("AdReward");
            }

            if (GUI.Button(new Rect(Screen.width * (0.20f), Screen.height * (0.65f), Screen.width * (0.25f), Screen.height * (0.14f)), "Instructions"))
            {
                focus.PlayButtonSound();
                focus.SetTarget(credits);
                //PlayerPrefs.DeleteAll();
            }

            if (GUI.Button(new Rect(Screen.width * (0.50f), Screen.height * (0.65f), Screen.width * (0.25f), Screen.height * (0.14f)), "Costumes"))
            {
                focus.PlayButtonSound();
                LevelQueue.LoadLevel("HorizontalTabs", true);
                //PlayerPrefs.DeleteAll();
            }
        }
        else
        {
            if (GUI.Button(new Rect(Screen.width * (0.35f), Screen.height * (0.25f), Screen.width * (0.25f), Screen.height * (0.14f)), "Start"))
            {
                focus.PlayButtonSound();
                focus.SetTarget(firstChapter);
            }

            if (GUI.Button(new Rect(Screen.width * (0.35f), Screen.height * (0.40f), Screen.width * (0.25f), Screen.height * (0.14f)), "Versus"))
            {
                focus.PlayButtonSound();
                Application.LoadLevel("MiniGameMenu");
            }

            if (GUI.Button(new Rect(Screen.width * (0.20f), Screen.height * (0.55f), Screen.width * (0.25f), Screen.height * (0.14f)), "Instructions"))
            {
                focus.PlayButtonSound();
                focus.SetTarget(credits);
                //PlayerPrefs.DeleteAll();
            }

            if (GUI.Button(new Rect(Screen.width * (0.50f), Screen.height * (0.55f), Screen.width * (0.25f), Screen.height * (0.14f)), "Costumes"))
            {
                focus.PlayButtonSound();
                LevelQueue.LoadLevel("HorizontalTabs", true);
                //PlayerPrefs.DeleteAll();
            }
        }



#if UNITY_EDITOR
        if (GUI.Button(new Rect(Screen.width * (0.85f), Screen.height * (0.05f), Screen.width * (0.1f), Screen.height * (0.11f)), "$$$"))
        {
            SaveFile.Instance().ModifySpiritInventory(SpiritType.ST_GREEN, 10);
            SaveFile.Instance().ModifySpiritInventory(SpiritType.ST_BLUE, 10);
            SaveFile.Instance().ModifySpiritInventory(SpiritType.ST_RED, 10);

            SaveFile.Instance().ModifyBlockInventory(BlockType.BT_WOOD, 10);
            SaveFile.Instance().ModifyBlockInventory(BlockType.BT_STONE, 10);
            SaveFile.Instance().ModifyBlockInventory(BlockType.BT_METAL, 10);
        }

        if (GUI.Button(new Rect(Screen.width * (0.85f), Screen.height * (0.17f), Screen.width * (0.1f), Screen.height * (0.11f)), "COINS"))
        {
            SIS.DBManager.IncreaseFunds("coins", 100);
        }

        if (GUI.Button(new Rect(Screen.width * (0.85f), Screen.height * (0.30f), Screen.width * (0.1f), Screen.height * (0.11f)), "RESET"))
        {
            SaveFile.Instance().SetSpiritInventory(SpiritType.ST_GREEN, 10);
            SaveFile.Instance().SetSpiritInventory(SpiritType.ST_BLUE, 10);
            SaveFile.Instance().SetSpiritInventory(SpiritType.ST_RED, 10);

            SaveFile.Instance().SetBlockInventory(BlockType.BT_WOOD, 10);
            SaveFile.Instance().SetBlockInventory(BlockType.BT_STONE, 10);
            SaveFile.Instance().SetBlockInventory(BlockType.BT_METAL, 10);
        }


#endif

        

	}

    public static void OnAdFinished(ShowResult result)
    {
        Debug.Log(result.ToString());
    }
}
