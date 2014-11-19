using UnityEngine;
using System.Collections;

public class Chapter : FocusPoint
{
	public GUISkin skin;

	public Chapter previousChapter, nextChapter;
	public FocusPoint mainMenu;

	public Vector2 menuTopLeftCorner = new Vector2(0.40f / 1f, 5.5f / 7.2f),
					menuSize = new Vector2(1f / 6f, 1f / 8f), 
					previousTopLeftCorner = new Vector2(0.2f / 1f, 5.5f / 7.2f),
					previousSize = new Vector2(1f / 6f, 1f / 8f), 
					nextTopLeftCorner = new Vector2(0.75f / 1f, 5.5f / 7.2f),
					nextSize = new Vector2(1f / 6f, 1f / 8f), 
					bannerTopLeftCorner = new Vector2(1f / 9f, -0.1f),
					bannerSize = new Vector2(7f / 9f, 4f / 9f);

	public string chapterTitle;
	public string[] unlockRequirements;

	private MenuFocus focus;
	private bool requirementsMet;

	void Start()
	{
		focus = FindObjectOfType<MenuFocus>() as MenuFocus;

		if (nextChapter)
		{
			requirementsMet = false;

			if (nextChapter.unlockRequirements.Length == 0)
			{
				requirementsMet = true;
			}
			else
			{
				for (int j = 0; j < nextChapter.unlockRequirements.Length && !requirementsMet; j++)
				{
					if (PlayerPrefs.HasKey(nextChapter.unlockRequirements[j]))
					{
						requirementsMet = true;
					}
				}
			}
		}

		if (gameObject.name != LevelQueue.menuContext || LevelQueue.startZoomed)
		{
			gameObject.SetActive(false);
		}
	}

	void OnGUI()
	{
		GUI.skin = skin;

		GUI.Label(new Rect(Screen.width * (bannerTopLeftCorner.x), Screen.height * (bannerTopLeftCorner.y),
								Screen.width * (bannerSize.x), Screen.height * (bannerSize.y)), chapterTitle);

		if (previousChapter)
		{
			if (GUI.Button(new Rect(Screen.width * (previousTopLeftCorner.x), Screen.height * (previousTopLeftCorner.y),
								Screen.width * (previousSize.x), Screen.height * (previousSize.y)), "Previous"))
			{
				focus.PlayButtonSound();
				focus.SetTarget(previousChapter);
			}
		}

		if (requirementsMet)
		{
			if (GUI.Button(new Rect(Screen.width * (nextTopLeftCorner.x), Screen.height * (nextTopLeftCorner.y),
								Screen.width * (nextSize.x), Screen.height * (nextSize.y)), "Next"))
			{
				focus.PlayButtonSound();
				focus.SetTarget(nextChapter);
			}
		}

		if (GUI.Button(new Rect(Screen.width * (menuTopLeftCorner.x), Screen.height * (menuTopLeftCorner.y),
								Screen.width * (menuSize.x), Screen.height * (menuSize.y)), "Main Menu"))
		{
			focus.PlayButtonSound();
			focus.SetTarget(mainMenu);
		}
	}
}
