//using UnityEngine;
//using System.Collections;

//public class LevelSelect : MonoBehaviour
//{
//    // declaring level select variables
//    public GUISkin mySkin;
//    public Chapter[] chapters;

//    private bool ActiveGUI;
//    private int currentChapter = 0;
//    private Vector3 velocityRef;

//    void Start()
//    {
//        ActiveGUI = true;

//        for (int j = 1; j < chapters.Length; j++)
//        {
//            chapters[j].gameObject.SetActive(false);
//        }
//    }
//    // Update is called once per frame
//    void Update()
//    {
//        Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, chapters[currentChapter].transform.position, ref velocityRef, 0.5f, 30f);

//        if (Vector3.Distance(Camera.main.transform.position, chapters[currentChapter].transform.position) < 0.1f && !ActiveGUI)
//        {
//            ActiveGUI = true;
//            chapters[currentChapter].gameObject.SetActive(true);
//        }
//    }

//    void OnGUI()
//    {
//        GUI.skin = mySkin;

//        // if Next chapter has not been selected display current chapters levels
//        if (ActiveGUI == true)
//        {
//            //if (GUI.Button(new Rect(Screen.width * (0.6f / 1f), Screen.height * (5.5f / 7.2f), Screen.width * (1f / 6f), Screen.height * (1f / 8f)), "Menu"))
//            //{
//                //Application.LoadLevel("MainMenu");
//            //}


//            if (currentChapter > 0)
//            {
//                if (GUI.Button(new Rect(Screen.width * (0.6f / 1f), Screen.height * (5.5f / 7.2f), Screen.width * (1f / 6f), Screen.height * (1f / 8f)), "Previous"))
//                {
//                    chapters[currentChapter].gameObject.SetActive(false);
//                    ActiveGUI = false;

//                    currentChapter--;
//                }
//            }

//            if (currentChapter < chapters.Length - 1)
//            {
//                bool requirementsMet = false;

//                if (chapters[currentChapter + 1].unlockRequirements.Length == 0)
//                {
//                    requirementsMet = true;
//                }
//                else
//                {
//                    for (int j = 0; j < chapters[currentChapter + 1].unlockRequirements.Length && !requirementsMet; j++)
//                    {
//                        if (PlayerPrefs.HasKey(chapters[currentChapter + 1].unlockRequirements[j]))
//                        {
//                            requirementsMet = true;
//                        }
//                    }
//                }

//                if (requirementsMet)
//                {
//                    if (GUI.Button(new Rect(Screen.width * (0.75f / 1f), Screen.height * (5.5f / 7.2f), Screen.width * (1f / 6f), Screen.height * (1f / 8f)), "Next"))
//                    {
//                        chapters[currentChapter].gameObject.SetActive(false);
//                        ActiveGUI = false;
//                        currentChapter++;
//                    }
//                }
//            }
//        }
//    }
//}
