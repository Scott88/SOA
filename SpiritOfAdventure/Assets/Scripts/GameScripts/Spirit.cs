//~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*
//Spirit.cs
//
//By: Nicholas MacDonald
//The half of the spirit AI that moves to specified targets and either triggers them
// or just fizzles. (other half of AI in FollowPlayer script)
//~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*

using UnityEngine;
using System.Collections;

public class Spirit : MonoBehaviour 
{
	public SpiritType type;

	private FollowPlayer follower;
	private Vector3 targetPosition;
	private Vector3 velocityRef, scaleRef;
	private bool movingToTarget, poofOnArrival;
	private SpiritReceiver spiritReceiver = null;
	private Vector3 targetScale;
	private bool flipped = false;
	private float volumeRef = 0f;
	private float targetVolume;

	private GameManager gameManager; 

	void Awake()
	{
		movingToTarget = false;
		follower = GetComponent<FollowPlayer>();
		targetVolume = audio.volume;
	}

	void Start()
	{
		targetScale = follower.targetScale;
		
	}

	//Convenience method that just lets the manager address itself to the spirits
	// so that the spirits don't need to find the manager themselves
	public void SetManager(GameManager manager)
	{
		gameManager = manager;
	}

	//Tells the spirit to move to this position and fizzle
	public void MoveHereAndPoof(Vector3 position)
	{
		follower.enabled = false;
		movingToTarget = true;
		poofOnArrival = true;
		targetPosition = position;
		collider2D.enabled = false;
		targetPosition.z = -30f;
	}

	//Tells the spirit to move to this position and trigger this receiver
	public void MoveHereAndTrigger(Vector3 position, SpiritReceiver receiver)
	{
		follower.enabled = false;
		movingToTarget = true;
		poofOnArrival = true;
		targetPosition = position;
		spiritReceiver = receiver;
		collider2D.enabled = false;
		targetPosition.z = -30f;
	}

	public void MoveHere(Vector3 position)
	{
		follower.enabled = false;
		movingToTarget = true;
		poofOnArrival = false;
		targetPosition = position;
		targetPosition.z = -30f;
	}

	public void Poof(SpiritReceiver receiver)
	{
		follower.enabled = true;
		follower.HideAway();
		movingToTarget = false;

		if (receiver)
		{
			receiver.Trigger(targetPosition);
		}
		else
		{
			//gameManager.scoreManager.SpiritFailed(type);
			gameManager.effectManager.PlayFizzleParticle(type, targetPosition);
		}

		particleSystem.Stop();
		follower.WarpOffscreen();
		audio.Stop();
	}

	public void Select()
	{
		follower.Follow();
		particleSystem.Play();
		collider2D.enabled = true;
		audio.Play ();
	}

	public void Deselect()
	{
		follower.HideAway();
		particleSystem.Stop();
		collider2D.enabled = false;
		audio.Stop();
	}

	//Here, if the spirit is moving to the target, once it gets close enough, it will either trigger the object
	// or play its fizzle particle effect. On the next frame, it warps back to its default position.
	void FixedUpdate()
	{
		if(movingToTarget)
		{
			transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocityRef, 0.4f, Mathf.Infinity, Time.fixedDeltaTime);

			if (velocityRef.x < 0 && targetScale.x > 0 ||
				velocityRef.x > 0 && targetScale.x < 0)
			{
				
				targetScale.x = -targetScale.x;
			}

			/*if (velocityRef.x < 0 && !flipped)
			{
				transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
				flipped = true;
			}

			if (velocityRef.x > 0 && flipped)
			{
				flipped = false;
				transform.rotation = Quaternion.identity;
			}*/

			transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref scaleRef, 0.2f, Mathf.Infinity, Time.fixedDeltaTime);
			audio.volume = Mathf.SmoothDamp(audio.volume, targetVolume, ref volumeRef, 0.2f, Mathf.Infinity, Time.fixedDeltaTime);

			if(Vector3.Distance (transform.position, targetPosition) < 1f && poofOnArrival)
			{
				Poof(spiritReceiver);
				spiritReceiver = null;
			}
		}
	}
}
