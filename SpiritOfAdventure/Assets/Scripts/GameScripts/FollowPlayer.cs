using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour
{
	private GameObject player;

	public Vector3 playerTarget;
	public Vector3 offscreenTarget;
	public Vector3 targetScale = Vector3.one;

	private Vector3 velocityRef;
	private Vector3 scaleRef;
	private float timeToReturn = 0.3f;
	private bool followingPlayer;
	private float volumeRef = 0f;
	private float targetVolume;

	void Awake()
	{
		player = GameObject.Find ("Player");
		targetVolume = GetComponent<AudioSource>().volume;
	}

	public void SetPlayer(GameObject p)
	{
        player = p;
	}

	public void Follow()
	{
		followingPlayer = true;
		timeToReturn = 0.3f;
	}

	public void HideAway()
	{
		followingPlayer = false;
	}

	void Start()
	{
		followingPlayer = false;
		GetComponent<AudioSource>().volume = 0;
	}

	public void WarpOffscreen()
	{
		transform.position = offscreenTarget + player.transform.position + new Vector3(-5f, 0f, 0f);
		transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
	}

	void FixedUpdate()
	{
		if (followingPlayer)
		{
			timeToReturn = 0.3f;

			Vector3 targetPos = player.transform.position + playerTarget;
			transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocityRef, 0.3f, Mathf.Infinity, Time.fixedDeltaTime);
			GetComponent<AudioSource>().volume = Mathf.SmoothDamp(GetComponent<AudioSource>().volume, targetVolume, ref volumeRef, 0.2f, Mathf.Infinity, Time.fixedDeltaTime);

			if (transform.localScale.x < targetScale.x - 0.001f)
			{
				transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref scaleRef, 0.3f, Mathf.Infinity, Time.fixedDeltaTime);
			}
		}
		else
		{
			if (transform.localScale.x > 0.001f)
			{
				Vector3 targetPos = player.transform.position + offscreenTarget;
				transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocityRef, timeToReturn, Mathf.Infinity, Time.fixedDeltaTime);
				timeToReturn -= Time.deltaTime * 0.5f;

				GetComponent<AudioSource>().volume = Mathf.SmoothDamp(GetComponent<AudioSource>().volume, 0f, ref volumeRef, 0.2f, Mathf.Infinity, Time.fixedDeltaTime);
				transform.localScale = Vector3.SmoothDamp(transform.localScale, new Vector3(), ref scaleRef, 0.3f, Mathf.Infinity, Time.fixedDeltaTime);
			}
			else
			{
				transform.position = player.transform.position + offscreenTarget;
			}
		}
	}
}
