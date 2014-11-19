using UnityEngine;
using System.Collections;

public class PointFloater : MonoBehaviour
{
	private int points = 0;
	private GameManager gameManager;

	void Awake()
	{
		gameManager = FindObjectOfType<GameManager>() as GameManager;
	}

	void Update()
	{
		if (points < 0)
		{
			transform.Translate(new Vector3(0f, -0.3f * Time.deltaTime, 0f));

			Color tempColor = guiText.material.color;
			tempColor.a -= Time.deltaTime;

			if (tempColor.a <= 0f)
			{
				Destroy(gameObject);
			}

			guiText.material.color = tempColor;
		}
		else if(points > 0)
		{
			transform.Translate(new Vector3(0f, 0.3f * Time.deltaTime, 0f));

			Color tempColor = guiText.material.color;
			tempColor.a += Time.deltaTime;

			if (tempColor.a > 1f)
			{
				gameManager.scoreManager.AddScore(points);
				Destroy(gameObject);
			}

			guiText.material.color = tempColor;
		}

		if (PauseButton.paused && guiText.enabled)
		{
			guiText.enabled = false;
		}
		else if(!PauseButton.paused && !guiText.enabled)
		{
			guiText.enabled = true;
		}
	}

	public void SetPoints(int p)
	{
		points = p;

		if (points < 0)
		{
			gameManager.scoreManager.AddScore(points);
			transform.position = new Vector3(0.98f, 0.88f, 0f);
			guiText.text = points.ToString();
		}
		else
		{
			transform.position = new Vector3(0.98f, 0.58f, 0f);
			Color tempColor = guiText.material.color;
			tempColor.a = 0f;
			guiText.material.color = tempColor;
			guiText.text = "+" + points.ToString();
		}
	}
}