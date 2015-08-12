using UnityEngine;
using System.Collections;

public class ActionFadeIn : SOAAction
{
	public float time = 1f;
	
	private bool fade;
	private Color col;


	void Start()
	{
		fade = false;
		col = GetComponent<Renderer>().material.color;
		col.a = 0f;
		GetComponent<Renderer>().material.color = col;
	}

	protected override void Update()
	{
		base.Update();
		if (fade == true)
		{
			col.a += Time.deltaTime / time;
			GetComponent<Renderer>().material.color = col;
			
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