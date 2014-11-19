using UnityEngine;
using System.Collections;

public class HintManager : MonoBehaviour
{
	public GameObject greenHintPref;
	public GameObject blueHintPref;
	public GameObject redHintPref;

	private GameObject currentHint;

	public void CreateHint(Vector3 position, Vector2 scale, SpiritType type)
	{
		if (currentHint)
		{
			DestroyHint();
		}

		if (type == SpiritType.ST_GREEN)
		{
			currentHint = (GameObject)Instantiate(greenHintPref, position, Quaternion.identity);
		}
		else if (type == SpiritType.ST_BLUE)
		{
			currentHint = (GameObject)Instantiate(blueHintPref, position, Quaternion.identity);
		}
		else if (type == SpiritType.ST_RED)
		{
			currentHint = (GameObject)Instantiate(redHintPref, position, Quaternion.identity);
		}

		currentHint.transform.localScale = new Vector3(scale.x, scale.y, 0);
	}

	public void DestroyHint()
	{
		if (currentHint)
		{
			currentHint.particleSystem.Stop();
			Destroy(currentHint, 1f);
		}
	}

	public bool HasActiveHint()
	{
		return currentHint;
	}
}
