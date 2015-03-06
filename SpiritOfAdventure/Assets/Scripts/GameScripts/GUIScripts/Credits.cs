using UnityEngine;
using System.Collections;

public class Credits : FocusPoint 
{
    public FocusPoint mainMenu;

    public GUISkin mySkin;

    public GameObject[] creditsObjects;

    private MenuFocus focus;

    private int index = 0;

    void Awake()
    {
        focus = FindObjectOfType<MenuFocus>();

        for (int j = 0; j < creditsObjects.Length; j++)
        {
            creditsObjects[j].SetActive(false);
        }

        if (creditsObjects.Length >= 1)
        {
            creditsObjects[0].SetActive(true);
        }

        if (gameObject.name != LevelQueue.menuContext)
        {
            gameObject.SetActive(false);
        }
    }

    void OnGUI()
    {
        GUI.skin = mySkin;

        if (index > 0)
        {
            if (GUI.Button(new Rect(Screen.width * (0.2f), Screen.height * (0.6f), Screen.width * (0.08f), Screen.height * (0.08f)), "<"))
            {
                creditsObjects[index].SetActive(false);
                index--;
                creditsObjects[index].SetActive(true);
            }
        }

        if (index < creditsObjects.Length - 1)
        {
            if (GUI.Button(new Rect(Screen.width * (0.75f), Screen.height * (0.6f), Screen.width * (0.08f), Screen.height * (0.08f)), ">"))
            {
                creditsObjects[index].SetActive(false);
                index++;
                creditsObjects[index].SetActive(true);
            }
        }

        if (GUI.Button(new Rect(Screen.width * (0.11f), Screen.height * (5.5f / 7.2f), Screen.width * (1f / 5f), Screen.height * (1f / 7f)), "Menu"))
        {
            focus.PlayButtonSound();
            focus.SetTarget(mainMenu);
        }
    }

}
