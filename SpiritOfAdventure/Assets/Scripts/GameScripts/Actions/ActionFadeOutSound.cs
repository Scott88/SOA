using UnityEngine;
using System.Collections;

public class ActionFadeOutSound : Action
{
	public AudioSource target;

	public float time = 1f;

	private bool fade;
	private float currentVolume;
	private float timer;

	protected override void Update()
	{
		base.Update();

		if (fade == true)
		{
			timer += Time.deltaTime;

			target.volume = Mathf.Lerp(currentVolume, 0f, timer / time);

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