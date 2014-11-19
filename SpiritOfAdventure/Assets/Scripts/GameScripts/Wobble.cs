using UnityEngine;
using System.Collections;

public class Wobble : MonoBehaviour
{
	public Vector3 wobbleDirection;
	public float frequency = 1f;

	private float current;

	void Start()
	{
		current = Random.Range(0f, 2 * Mathf.PI);
	}

	void Update()
	{
		float lastValue = Mathf.Sin(current);

		current += Time.deltaTime * frequency;

		if (current >= 2 * Mathf.PI)
		{
			current -= 2 * Mathf.PI;
		}

		float nextValue = Mathf.Sin(current);

		transform.Translate(wobbleDirection * (nextValue - lastValue));
	}
}
