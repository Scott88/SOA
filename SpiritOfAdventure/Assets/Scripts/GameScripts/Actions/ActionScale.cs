using UnityEngine;
using System.Collections;

public class ActionScale : Action
{
	public Vector3 scale;
	public float time;

	private float timer;
	private float speed;

	private bool isScaleing = false;

	void Start()
	{
		timer = time;
		speed = 1 / time;
		Vector3 scale = transform.localScale;
		transform.localScale = scale;
	}

	protected override void Activate()
	{
		isScaleing = true;
	}

	protected override void Update()
	{
		base.Update();
		if (isScaleing)
		{
			transform.localScale += scale * Time.deltaTime * speed;
			timer -= Time.deltaTime;

			if (timer <= 0f)
			{
				ActionComplete();
				isScaleing = false;
			}
		}
	}
}