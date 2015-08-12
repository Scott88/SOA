using UnityEngine;
using System.Collections;

public class SelfDeleter : MonoBehaviour
{
	public float lifeTime = 2f;
	public bool fadeOut = false;

	void Update()
	{
		lifeTime -= Time.deltaTime;

		if (fadeOut == true && lifeTime < 1f)
		{
			Color tempColor = GetComponent<Renderer>().material.color;
			tempColor.a = lifeTime;
			GetComponent<Renderer>().material.color = tempColor;
		}

		if (lifeTime <= 0)
		{
			Destroy(gameObject);
		}
	}
}
