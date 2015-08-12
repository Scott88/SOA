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
			GetComponent<AudioSource>().clip = sounds[Random.Range(0, sounds.Length - 1)];
			GetComponent<AudioSource>().volume = Random.Range(minVolume, maxVolume);

			if (randomizePan)
			{
				GetComponent<AudioSource>().panStereo = Random.Range(-1f, 1f);
			}

			GetComponent<AudioSource>().Play();

			timer = Random.Range(minTime, maxTime);
		}
	}
}
