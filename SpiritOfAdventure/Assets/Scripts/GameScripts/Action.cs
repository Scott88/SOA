//~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*
//Action.cs
//
//By: Nicholas MacDonald
//Base class for a single event
//~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*

using UnityEngine;

public abstract class Action : MonoBehaviour
{
	public float delay;
	public AudioClip[] activationSounds;
	public Action[] resultingActions = null;

	public bool triggered { get; set; }

	private bool primed = false;
	private bool hasPosition = false;
	private Vector3 pendingPosition;
    private float Triggertimer;

	void Start()
	{
		triggered = false;
	}

	//Gets the action ready for activation.
	public void Prime()
	{
		primed = true;
		hasPosition = false;
        Triggertimer = 0;
	}

	public void Prime(Vector3 position)
	{
		primed = true;
		hasPosition = true;
		pendingPosition = position;
        Triggertimer = 0;
	}

	protected virtual void Update()
	{
		//If the action has been primed and it hasn't already been triggered
		if (primed)
		{
			Triggertimer += Time.deltaTime;

			//If we've waited the full duration of the delay
			if (Triggertimer >= delay)
			{
				//We activate our action with the corresponding position (if it has one)
				if (hasPosition)
				{
					Activate(pendingPosition);
				}
				else
				{
					Activate();
				}

				if (activationSounds.Length > 0)
				{
					audio.clip = activationSounds[Random.Range(0, activationSounds.Length - 1)];
					audio.Play();
				}

                primed = false;
				triggered = true;
			}
		}
	}

	//This method contains child specific code that corresponds to each action
	abstract protected void Activate();

	//Same as above except for when a trigger has a contextual position associated with it
	virtual protected void Activate(Vector3 position)
	{
		Activate();
	}

	//When the action is complete, we prime all of the resulting actions
	protected void ActionComplete()
	{
		for (int j = 0; j < resultingActions.Length; j++)
		{
			resultingActions[j].Prime();
		}
	}

	//In the editor, we find out which actions this action is responsible for.
	protected virtual void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.red;

		bool missingActions = false;

		if (resultingActions != null)
		{
			for (int j = 0; j < resultingActions.Length; j++)
			{
				if (resultingActions[j] != null)
				{
					Gizmos.DrawLine(transform.position, resultingActions[j].transform.position);
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

		//I kind of wish I could have put in a system that shows what trigger/action actually triggers THIS action, but I never really
		// found an eloquent way of doing it. Originally I wanted it so that triggers would just send themselves to the actions as
		// their "parent" but that transfer never actually occurs when just looking in the editor. The only other option I could think
		// of was getting a list of EVERY trigger and EVERY action just to find which one parents this one and that sounds really slow
		// and unnecessary.
	}
}

