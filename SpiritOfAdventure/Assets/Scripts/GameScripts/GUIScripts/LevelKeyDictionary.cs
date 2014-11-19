using UnityEngine;
using System.Collections.Generic;

public class LevelKeyDictionary : MonoBehaviour
{
	public LevelKey[] keyList;

	private static Dictionary<int, string> keyDictionary;
	public static bool created = false;

	void Start()
	{
		for (int j = 0; j < keyList.Length; j++)
		{
			keyDictionary.Add(keyList[j].sceneNumber, keyList[j].key);
		}

		created = true;

		Destroy(gameObject);
	}

	public static bool HasLevelKey(int sceneNumber)
	{
		return keyDictionary.ContainsKey(sceneNumber);
	}

	public static string GetLevelKey(int sceneNumber)
	{
		return keyDictionary[sceneNumber];
	}
}
