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
			Color tempColor = renderer.material.color;
			tempColor.a = lifeTime;
			renderer.material.color = tempColor;
		}

		if (lifeTime <= 0)
		{
			Destroy(gameObject);
		}
	}
}
