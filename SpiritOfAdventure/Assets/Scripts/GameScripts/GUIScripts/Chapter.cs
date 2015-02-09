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
			requirementsMet = true;

			if (nextChapter.unlockRequirements.Length > 0)
			{
				for (int j = 0; j < nextChapter.unlockRequirements.Length && requirementsMet; j++)
				{
					if (SaveFile.Instance().GetStars(nextChapter.unlockRequirements[j]) == 0)
					{
						requirementsMet = false;
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
								Screen.width * (menuSize.x), Screen.height * (menuSize.y)), "Menu"))
		{
			focus.PlayButtonSound();
			focus.SetTarget(mainMenu);
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		
		float length = (float)Screen.width / 100f;
		float height = (float)Screen.height / 100f;
		
		Vector3 topLeft = new Vector3(0f, 0f, 0f);
		Vector3 topRight = new Vector3(length, 0f, 0f);
		Vector3 bottomRight = new Vector3(length, height, 0f);
		Vector3 bottomLeft = new Vector3(0f, height, 0f);
		
		topLeft.x = length * menuTopLeftCorner.x;
		topLeft.y = height - height * menuTopLeftCorner.y;
		
		topRight.x = length * menuTopLeftCorner.x + length * menuSize.x;
		topRight.y = topLeft.y;
		
		bottomLeft.x = topLeft.x;
		bottomLeft.y = height - (height * menuTopLeftCorner.y + height * menuSize.y);
		
		bottomRight.x = topRight.x;
		bottomRight.y = bottomLeft.y;
		
		Gizmos.DrawLine(topLeft, topRight);
		Gizmos.DrawLine(topRight, bottomRight);
		Gizmos.DrawLine(bottomRight, bottomLeft);
		Gizmos.DrawLine(bottomLeft, topLeft);
		
		topLeft.x = length * previousTopLeftCorner.x;
		topLeft.y = height - height * previousTopLeftCorner.y;
		
		topRight.x = length * previousTopLeftCorner.x + length * previousSize.x;
		topRight.y = topLeft.y;
		
		bottomLeft.x = topLeft.x;
		bottomLeft.y = height - (height * previousTopLeftCorner.y + height * previousSize.y);
		
		bottomRight.x = topRight.x;
		bottomRight.y = bottomLeft.y;
		
		Gizmos.DrawLine(topLeft, topRight);
		Gizmos.DrawLine(topRight, bottomRight);
		Gizmos.DrawLine(bottomRight, bottomLeft);
		Gizmos.DrawLine(bottomLeft, topLeft);

		topLeft.x = length * nextTopLeftCorner.x;
		topLeft.y = height - height * nextTopLeftCorner.y;
		
		topRight.x = length * nextTopLeftCorner.x + length * nextSize.x;
		topRight.y = topLeft.y;
		
		bottomLeft.x = topLeft.x;
		bottomLeft.y = height - (height * nextTopLeftCorner.y + height * nextSize.y);
		
		bottomRight.x = topRight.x;
		bottomRight.y = bottomLeft.y;
		
		Gizmos.DrawLine(topLeft, topRight);
		Gizmos.DrawLine(topRight, bottomRight);
		Gizmos.DrawLine(bottomRight, bottomLeft);
		Gizmos.DrawLine(bottomLeft, topLeft);

		topLeft.x = length * bannerTopLeftCorner.x;
		topLeft.y = height - height * bannerTopLeftCorner.y;
		
		topRight.x = length * bannerTopLeftCorner.x + length * bannerSize.x;
		topRight.y = topLeft.y;
		
		bottomLeft.x = topLeft.x;
		bottomLeft.y = height - (height * bannerTopLeftCorner.y + height * bannerSize.y);
		
		bottomRight.x = topRight.x;
		bottomRight.y = bottomLeft.y;
		
		Gizmos.DrawLine(topLeft, topRight);
		Gizmos.DrawLine(topRight, bottomRight);
		Gizmos.DrawLine(bottomRight, bottomLeft);
		Gizmos.DrawLine(bottomLeft, topLeft);
		
		//Distant and very confusing memories.
		
		/*Gizmos.color = new Color(0.804f, 0.498f, 0.196f);
		Vector3 center = new Vector3(bronzeTopLeft.x * length + bronzeSize * length * 0.5f, -(bronzeTopLeft.y * height + bronzeSize * height * 0.5f) + height, 0);
		Gizmos.DrawCube(center, new Vector3(bronzeSize * height, bronzeSize * height, 0.01f));

		Gizmos.color = new Color(0.753f, 0.753f, 0.753f);
		center = new Vector3(silverTopLeft.x * length + silverSize * length * 0.5f, -(silverTopLeft.y * height + silverSize * height * 0.5f) + height, 0);
		Gizmos.DrawCube(center, new Vector3(silverSize * height, silverSize * height, 0.01f));

		Gizmos.color = new Color(1f, 0.843f, 0f);
		center = new Vector3(goldTopLeft.x * length + goldSize * length * 0.5f, -(goldTopLeft.y * height + goldSize * height * 0.5f) + height, 0);
		Gizmos.DrawCube(center, new Vector3(goldSize * height, goldSize * height, 0.01f));*/
	}
}