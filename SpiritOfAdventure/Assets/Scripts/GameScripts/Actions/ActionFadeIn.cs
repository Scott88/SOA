using UnityEngine;
using System.Collections;

public class ActionFadeIn : Action
{
	public float time = 1f;
	
	private bool fade;
	private Color col;
	
	void Start()
	{
		fade = false;
		col = renderer.material.color;
		col.a = 0f;
		renderer.material.color = col;
	}

	protected override void Update()
	{
		base.Update();
		if (fade == true)
		{
			col.a += Time.deltaTime / time;
			renderer.material.color = col;
			
			if (col.a >= 1f)
			{
				fade = false;
				ActionComplete();
			}
		}
	}

	protected override void Activate()
	{
		fade = true;
	}
	
}