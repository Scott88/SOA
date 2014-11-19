using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour 
{
	
	void Start () 
	{
	
	}

	// Update is called once per frame
	void Update()
	{
		if (Application.GetStreamProgressForLevel("level01") == 1)
		{
			Application.LoadLevel("level01");
		}
	}


}
