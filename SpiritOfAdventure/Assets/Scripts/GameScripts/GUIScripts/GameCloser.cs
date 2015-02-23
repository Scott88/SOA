using UnityEngine;
using System.Collections;

public class GameCloser : MonoBehaviour
{
	private static bool created = false;

	void Awake()
	{
		if (!created)
		{
			DontDestroyOnLoad(this.gameObject);
			created = true;
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	void OnApplicationQuit()
    {
        SaveFile.Instance().SaveToXML();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveFile.Instance().SaveToXML();
        }
    }
}
