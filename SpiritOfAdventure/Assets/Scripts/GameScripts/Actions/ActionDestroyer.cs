﻿using UnityEngine;
using System.Collections;

public class ActionDestroyer : SOAAction
{
	public float time = 1f;
	public bool fadeOut = false;

	private bool fading;
	private Color col;

	void Start()
	{
		fading = false;

		if (fadeOut)
		{
			col = GetComponent<Renderer>().material.color;
		}
	}

	protected override void Update()
	{
		base.Update();
		if (fading == true)
		{
			col.a -= Time.deltaTime / time;
			GetComponent<Renderer>().material.color = col;

			if (col.a <= 0f)
			{
				fading = false;
				ActionComplete();
				Destroy(gameObject);
			}
		}
	}

	protected override void Activate()
	{
		if (fadeOut)
		{
			fading = true;
		}
		else
		{
			ActionComplete();
			Destroy(gameObject);
		}
	}

}