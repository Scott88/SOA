using UnityEngine;
using System.Collections;

public class ActionHintDestroyer : Action
{
	public ActionHintCreator hinterToStop;

	private GameManager gameManager;

	void Awake()
	{
		gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
	}

	protected override void Activate()
	{
		if (hinterToStop)
		{
			if (!gameManager.hintManager.HasActiveHint())
			{
				hinterToStop.enabled = false;
			}
			else
			{
				gameManager.hintManager.DestroyHint();
			}
		}
		else
		{
			gameManager.hintManager.DestroyHint();
		}

		ActionComplete();
	}
}