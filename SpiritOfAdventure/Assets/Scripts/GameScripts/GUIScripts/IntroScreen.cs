using UnityEngine;
using System.Collections;

public class IntroScreen : MonoBehaviour
{
	public float fadeInTime, displayTime, fadeOutTime;

	private Color alphaController;

	void Start()
	{
		alphaController = renderer.material.color;
		alphaController.a = 0;
		renderer.material.color = alphaController;
		StartCoroutine(Go());
		Time.timeScale = 1;
	}



	IEnumerator Go()
	{
		float timer = fadeInTime;

		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			alphaController.a = 1 - timer / fadeInTime;
			renderer.material.color = alphaController;
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
			alphaController.a = timer / fadeOutTime;
			renderer.material.color = alphaController;
			yield return 0;
		}

		Application.LoadLevel("CutScene");
	}

}
