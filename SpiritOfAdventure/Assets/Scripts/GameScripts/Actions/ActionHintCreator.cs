using UnityEngine;
using System.Collections;

public class ActionHintCreator : SOAAction
{
	public GUISkin skin;
	public Vector3 offset;
	public Vector2 scale = new Vector2(1f, 1f);
	public SpiritType type;
	public bool useButtonInstead = true;

	private bool showButton = false;
	private GameManager gameManager;

	void Awake()
	{
		gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
	}

	protected override void Activate()
	{
		if (!useButtonInstead)
		{
			gameManager.hintManager.CreateHint(transform.position + offset, scale, type);
		}
		else
		{
			showButton = true;
		}

		ActionComplete();
	}

	void OnGUI()
	{
		if (showButton && !PauseButton.paused)
		{
			GUI.skin = skin;

			if (GUI.Button(new Rect(Screen.width * (0.85f), Screen.height * (0.85f), Screen.width * (0.1f), Screen.height * (0.1f)), "HINT"))
			{
				gameManager.hintManager.CreateHint(transform.position + offset, scale, type);
				showButton = false;
			}
		}
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();

		if (type == SpiritType.ST_GREEN)
		{
			Gizmos.color = Color.green;
		}
		else if (type == SpiritType.ST_BLUE)
		{
			Gizmos.color = Color.cyan;
		}
		else if (type == SpiritType.ST_RED)
		{
			Gizmos.color = Color.red;
		}

		Vector3 point1 = new Vector3();
		Vector3 point2 = new Vector3();		

		float radians = 0;

		point2.x = Mathf.Sin(radians) * scale.x;
		point2.y = Mathf.Cos(radians) * scale.y;

		Vector3 circleCenter = transform.position + offset;

		for (int j = 0; j < 50; j++)
		{
			point1 = point2;

			radians += (Mathf.PI * 2f) / 50;

			point2.x = Mathf.Sin(radians) * scale.x;
			point2.y = Mathf.Cos(radians) * scale.y;

			Gizmos.DrawLine(point1 + circleCenter, point2 + circleCenter);
		}
	}
}