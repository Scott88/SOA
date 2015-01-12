using UnityEngine;
using System.Collections;

public class CutScene : SOAAction
{
	public float fadeInTime, displayTime, fadeOutTime;

	public AudioSource target;
	
	public float time = 1f;
	
	public bool fade;
	private float currentVolume;
	private float timer;
	
	void Start()
	{
		StartCoroutine(Go());
		Time.timeScale = 1;
		currentVolume = target.volume;
		//PlayerPrefs.DeleteAll ();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown (0)) 
		{
			Application.LoadLevel("MainMenu");
		}

		if (fade == true)
		{
			timer += Time.deltaTime;
			
			target.volume = Mathf.Lerp(currentVolume, 0f, timer / time);
			
			if (timer >= time)
			{
				fade = false;
			}
		}
	}

	
	
	IEnumerator Go()
	{
		float timer = fadeInTime;


		
		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			yield return 0;
		}
		
		timer = displayTime;
		
		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			yield return 0;
		}
		
		timer = fadeOutTime;
		
		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			yield return 0;
		}
		
		Application.LoadLevel("MainMenu");
	}

	protected override void Activate()
	{

	}
	
}
