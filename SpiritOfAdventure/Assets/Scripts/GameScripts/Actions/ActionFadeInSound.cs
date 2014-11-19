using UnityEngine;
using System.Collections;

public class ActionFadeInSound : Action
{
	public AudioSource target;

	public float time = 1f;

	public bool overrideVolume;
	public float overrideValue;

	private bool fade;
	private float originalVolume, currentVolume;
	private float timer;

	void Start()
	{
		if (!overrideVolume)
		{
			originalVolume = target.volume;
		}
		else
		{
			originalVolume = overrideValue;
		}
	}

	protected override void Update()
	{
		base.Update();

		if (fade == true)
		{
			timer += Time.deltaTime;

			target.volume = Mathf.Lerp(currentVolume, originalVolume, timer / time);

			if (timer >= time)
			{
				fade = false;
				ActionComplete();
			}
		}
	}

	protected override void Activate()
	{
		currentVolume = target.volume;
		timer = 0;
		fade = true;
	}

}