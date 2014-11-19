using UnityEngine;
using System.Collections;

public class TriggerList : Triggerable
{

	public Triggerable[] triggers;
	
	// Update is called once per frame
	void Update ()
	{
		TestTriggers ();
	}

	void TestTriggers()
	{
		if (!hasTriggered)
		{
			for (int j = 0; j < triggers.Length; j++) {
					if (!triggers [j].HasTriggered ()) {
							return;
					}
			}

			Trigger ();
		}
	}
}
