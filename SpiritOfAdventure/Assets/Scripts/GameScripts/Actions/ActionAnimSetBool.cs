using UnityEngine;
using System.Collections;

public class ActionAnimSetBool : SOAAction
{
	public string variable;
	public bool value;
	private Animator animator;

	void Start()
	{
		animator = GetComponent<Animator>();
	}

	protected override void Activate()
	{
		animator.SetBool(variable, value);
		ActionComplete();
	}
}
