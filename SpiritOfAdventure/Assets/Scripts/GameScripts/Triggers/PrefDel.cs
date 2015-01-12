using UnityEngine;
using System.Collections;

public class PrefDel : SOAAction
{
	public bool pA;
	public bool pB;
	public bool pC;
	public bool ALL;

	public int pathA;
	public int pathB;
	public int pathC;

	private bool Del;
	
	// Use this for initialization
	void Awake () 
	{

		pathA = PlayerPrefs.GetInt ("pa");
		pathB = PlayerPrefs.GetInt ("pb");
		pathC = PlayerPrefs.GetInt ("pc");
	}
	
	protected override void Activate ()
	{
		Del = true;

		if (Del = true)
		{
			if (ALL = true)
			{
				PlayerPrefs.DeleteKey("pa");
				PlayerPrefs.DeleteKey("pb");
				PlayerPrefs.DeleteKey("pC");
				Del = false;
			}
			if (pA == true)
			{
				PlayerPrefs.DeleteKey("pa");
			}
			if (pB == true)
			{
				PlayerPrefs.DeleteKey("pb");
			}
			if (pC == true)
			{
				PlayerPrefs.DeleteKey("pc");
			}
			
			Del = false;
		}
		
	}
}