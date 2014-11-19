using UnityEngine;
using System.Collections;

public class NullLevelEscape : MonoBehaviour
{
	private float timer = 6f;

	void Update()
	{
		timer -= Time.deltaTime;

		if (timer <= 0f)
		{
			Application.LoadLevel("MainMenu");
		}
	}
}
