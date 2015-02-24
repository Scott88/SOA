using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    public TextMesh hintDisplay;

	void Start()
	{
        hintDisplay.text = FindObjectOfType<HintList>().GetRandomHint();

		if (LevelQueue.UsesInt())
		{
			Application.LoadLevel(LevelQueue.GetNextLevel());
		}
		else
		{
			Application.LoadLevel(LevelQueue.GetNextLevelString());
		}
	}
}