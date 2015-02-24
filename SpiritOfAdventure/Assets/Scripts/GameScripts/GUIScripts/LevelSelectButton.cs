using UnityEngine;
using System.Collections;

public class LevelSelectButton : MonoBehaviour
{
	public GUISkin skin;
	public GameObject bronzeStar, silverStar, goldStar;
	public Vector2 topLeftCorner, size;
    public Vector2 scoreTopLeft, scoreSize;
	public string levelTitle;
	public string[] unlockRequirements;
	public string sceneToLoad = "NullLevel";

	private bool requirementsMet;
	private int starCount;
	private MenuFocus focus;

	void Start()
	{
		focus = FindObjectOfType<MenuFocus>() as MenuFocus;

		requirementsMet = true;

		if (unlockRequirements.Length > 0)
		{
			for (int j = 0; j < unlockRequirements.Length && requirementsMet; j++)
			{
				if (SaveFile.Instance().GetLevelStars(unlockRequirements[j]) == 0)
				{
					requirementsMet = false;
				}
			}
		}

		HideStars();

        starCount = SaveFile.Instance().GetLevelStars(sceneToLoad);

        ShowStars();
	}

	void OnGUI()
	{
		GUI.skin = skin;

		if (GUI.Button(new Rect(Screen.width * (topLeftCorner.x), Screen.height * (topLeftCorner.y),
								Screen.width * (size.x), Screen.height * (size.y)), levelTitle))
		{
			if (requirementsMet)
			{
				focus.PlayButtonSound();
				GameManager.ResetGame();
				LevelQueue.menuContext = transform.parent.gameObject.name;
				LevelQueue.startZoomed = true;
				LevelQueue.LoadLevel(sceneToLoad);
			}
		}

        GUI.Label(new Rect(Screen.width * (scoreTopLeft.x), Screen.height * (scoreTopLeft.y),
                                Screen.width * (scoreSize.x), Screen.height * (scoreSize.y)), SaveFile.Instance().GetScore(sceneToLoad).ToString());
	}

	void OnDisable()
	{
		HideStars();
	}

	void OnEnable()
	{
		ShowStars();
	}

	void HideStars()
	{
		if (bronzeStar)
		{
			bronzeStar.SetActive(false);
		}

		if (silverStar)
		{
			silverStar.SetActive(false);
		}

		if (goldStar)
		{
			goldStar.SetActive(false);
		}
	}

	void ShowStars()
	{
		if (starCount >= 1)
		{
			bronzeStar.SetActive(true);
		}

		if (starCount >= 2)
		{
			silverStar.SetActive(true);
		}

		if (starCount == 3)
		{
			goldStar.SetActive(true);
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

		Gizmos.DrawLine(topLeft, topRight);
		Gizmos.DrawLine(topRight, bottomRight);
		Gizmos.DrawLine(bottomRight, bottomLeft);
		Gizmos.DrawLine(bottomLeft, topLeft);

		topLeft.x = length * topLeftCorner.x;
		topLeft.y = height - height * topLeftCorner.y;

		topRight.x = length * topLeftCorner.x + length * size.x;
		topRight.y = topLeft.y;

		bottomLeft.x = topLeft.x;
		bottomLeft.y = height - (height * topLeftCorner.y + height * size.y);

		bottomRight.x = topRight.x;
		bottomRight.y = bottomLeft.y;

		Gizmos.DrawLine(topLeft, topRight);
		Gizmos.DrawLine(topRight, bottomRight);
		Gizmos.DrawLine(bottomRight, bottomLeft);
		Gizmos.DrawLine(bottomLeft, topLeft);

        Gizmos.color = Color.cyan;

        topLeft.x = length * scoreTopLeft.x;
        topLeft.y = height - height * scoreTopLeft.y;

        topRight.x = length * scoreTopLeft.x + length * scoreSize.x;
        topRight.y = topLeft.y;

        bottomLeft.x = topLeft.x;
        bottomLeft.y = height - (height * scoreTopLeft.y + height * scoreSize.y);

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
