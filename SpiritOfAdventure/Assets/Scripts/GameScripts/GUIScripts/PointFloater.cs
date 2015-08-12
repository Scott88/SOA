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

			Color tempColor = GetComponent<GUIText>().material.color;
			tempColor.a -= Time.deltaTime;

			if (tempColor.a <= 0f)
			{
				Destroy(gameObject);
			}

			GetComponent<GUIText>().material.color = tempColor;
		}
		else if(points > 0)
		{
			transform.Translate(new Vector3(0f, 0.3f * Time.deltaTime, 0f));

			Color tempColor = GetComponent<GUIText>().material.color;
			tempColor.a += Time.deltaTime;

			if (tempColor.a > 1f)
			{
				gameManager.scoreManager.AddScore(points);
				Destroy(gameObject);
			}

			GetComponent<GUIText>().material.color = tempColor;
		}

		if (PauseButton.paused && GetComponent<GUIText>().enabled)
		{
			GetComponent<GUIText>().enabled = false;
		}
		else if(!PauseButton.paused && !GetComponent<GUIText>().enabled)
		{
			GetComponent<GUIText>().enabled = true;
		}
	}

	public void SetPoints(int p)
	{
		points = p;

		if (points < 0)
		{
			gameManager.scoreManager.AddScore(points);
			transform.position = new Vector3(0.98f, 0.88f, 0f);
			GetComponent<GUIText>().text = points.ToString();
		}
		else
		{
			transform.position = new Vector3(0.98f, 0.58f, 0f);
			Color tempColor = GetComponent<GUIText>().material.color;
			tempColor.a = 0f;
			GetComponent<GUIText>().material.color = tempColor;
			GetComponent<GUIText>().text = "+" + points.ToString();
		}
	}
}