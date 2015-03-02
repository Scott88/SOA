using UnityEngine;
using System.Collections;


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

	void OnGUI () 
	{
		GUI.skin = mySkin;


		GUI.Label(new Rect(Screen.width * (0.19f), Screen.height * (0.05f), Screen.width * (0.6f), Screen.height * (0.2f)), "Spirit Of Adventure");



			if(GUI.Button(new Rect(Screen.width * (0.35f),Screen.height * (0.32f),Screen.width * (0.25f), Screen.height * (1f/7f)), "Start"))
			{
				focus.PlayButtonSound();
				focus.SetTarget(firstChapter);
			}
			
			if(GUI.Button(new Rect(Screen.width * (0.2f),Screen.height * (0.55f),Screen.width * (0.25f), Screen.height * (1f/7f)), "Instructions")) 
			{
				focus.PlayButtonSound();
				focus.SetTarget(credits);
				//PlayerPrefs.DeleteAll();
			}
			
			if(GUI.Button(new Rect(Screen.width * (0.53f),Screen.height * (0.55f),Screen.width * (0.25f), Screen.height * (1f/7f)), "Costumes")) 
			{
				focus.PlayButtonSound();
				LevelQueue.LoadLevel("HorizontalTabs", true);
				//PlayerPrefs.DeleteAll();
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

            //if (GUI.Button(new Rect(Screen.width * (0.53f), Screen.height * (0.32f), Screen.width * (0.25f), Screen.height * (1f / 7f)), "DESTRUCTORNATOR"))
            //{

            //    focus.PlayButtonSound();
            //    Application.LoadLevel("MiniGameMenu");
            //}

	}
}
