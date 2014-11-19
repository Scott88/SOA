using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
	void Start()
	{
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