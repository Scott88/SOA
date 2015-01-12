using UnityEngine;
using System.Collections;

public class ExternalCredits : FocusPoint
{
	public GUISkin mySkin;

	public FocusPoint mainMenu;

	private MenuFocus focus;
	protected Animator animator;



	void Start()
	{
		animator = GetComponent<Animator> ();
		focus = FindObjectOfType<MenuFocus>() as MenuFocus;

		if (gameObject.name != LevelQueue.menuContext)

		{

			gameObject.SetActive(false);
		}
	}

	void OnGUI()
	{

		GUI.skin = mySkin;
		animator.SetBool("play",true);

		// Make a background box
		//GUI.Box(new Rect(10,10,100,90), "Loader Menu");

		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if (GUI.Button(new Rect(Screen.width * (0.11f), Screen.height * (5.5f / 7.2f), Screen.width * (1f / 5f), Screen.height * (1f / 7f)), "Menu"))
		{
			focus.PlayButtonSound();
			focus.SetTarget(mainMenu);

		}

	}
}
