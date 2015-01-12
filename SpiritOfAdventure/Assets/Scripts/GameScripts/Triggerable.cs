//~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*
//Trigger.cs
//
//By: Nicholas MacDonald
//Base class for objects that activate a list of actions when interacted with
// in a specified way.
//~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*

using UnityEngine;

public class Triggerable : MonoBehaviour
{
	public SOAAction[] actionList;
    public bool onlyTriggerOnce = false;

	protected bool hasTriggered;

	//Called when the object was interacted with, causing all of the actions to get primed
	virtual public void Trigger()
	{
		if ((!hasTriggered && onlyTriggerOnce) || !onlyTriggerOnce)
		{	
			for (int j = 0; j < actionList.Length; j++)
			{
				actionList[j].Prime();
			}

			hasTriggered = true;
		}
	}

	public bool HasTriggered()
	{
		return hasTriggered;
	}


	virtual public void Trigger(Vector3 position)
	{
        if ((!hasTriggered && onlyTriggerOnce) || !onlyTriggerOnce)
		{
			for (int j = 0; j < actionList.Length; j++)
			{
				actionList[j].Prime(position);
			}

			hasTriggered = true;
		}
	}

	//See Action.OnDrawGizmosSelected
	protected virtual void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.red;

		bool missingActions = false;

		if (actionList != null)
		{
			for (int j = 0; j < actionList.Length; j++)
			{
				if (actionList[j] != null)
				{
					Gizmos.DrawLine(transform.position, actionList[j].transform.position);
				}
				else
				{
					missingActions = true;
				}
			}
		}

		if (missingActions)
		{
			Debug.LogWarning("The Trigger/Action inside Game Object: " + gameObject.name + " has resulting actions that are null.", gameObject);
		}
	}
}
