using UnityEngine;
using System.Collections;

public class CreditsPlay : FocusPoint {

	protected Animator animator;

	private MenuFocus focus;

	public FocusPoint mainMenu, credits;

	void Start ()
	{
		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
}
