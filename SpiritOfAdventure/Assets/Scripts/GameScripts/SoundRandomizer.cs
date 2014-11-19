using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundRandomizer : MonoBehaviour
{
	public AudioClip[] sounds;
	public float minTime, maxTime;
	[Range(0f, 1f)] 
	public float minVolume, maxVolume;
	public bool randomizePan = true;

	private float timer;

	void Start()
	{
		timer = Random.Range(minTime, maxTime);
	}

	void Update()
	{
		timer -= Time.deltaTime;

		if (timer <= 0f)
		{
			audio.clip = sounds[Random.Range(0, sounds.Length - 1)];
			audio.volume = Random.Range(minVolume, maxVolume);

			if (randomizePan)
			{
				audio.pan = Random.Range(-1f, 1f);
			}

			audio.Play();

			timer = Random.Range(minTime, maxTime);
		}
	}
}
