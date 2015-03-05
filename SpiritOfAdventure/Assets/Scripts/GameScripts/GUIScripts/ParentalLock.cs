using UnityEngine;
using System.Collections;

public class ParentalLock : MonoBehaviour
{
    public GUISkin mySkin;

    public TextMesh mathText;

    public int maxAttempts = 3;

    public GameObject store, lilyBucksStore;
    public GameObject[] otherMenus;

    public GameObject lockPrompt, failPrompt, lockedOut;

    public static int attempts = 0;

    private int correctAnswer;

    private int[] answers;

    private bool ready = false;

    public void SetUpTest()
    {
        store.SetActive(false);

        if (attempts < maxAttempts)
        {        
            lockPrompt.SetActive(true);

            int firstNumber, secondNumber;

            firstNumber = Random.Range(4, 10);
            secondNumber = Random.Range(11, 16);

            mathText.text = firstNumber + " x " + secondNumber + " =";

            correctAnswer = firstNumber * secondNumber;

            answers = new int[8];

            for (int j = 0; j < 8; j++)
            {
                bool good = false;

                while (!good)
                {
                    good = true;

                    answers[j] = Random.Range(44, 136);

                    if (answers[j] == correctAnswer)
                    {
                        good = false;
                    }

                    if (good)
                    {
                        for (int k = 0; k < j; k++)
                        {
                            if (answers[j] == answers[k])
                            {
                                good = false;
                            }
                        }
                    }
                }
            }

            answers[Random.Range(0, 8)] = correctAnswer;
            ready = true;
        }
        else
        {
            StartCoroutine(ShowPopup(lockedOut));
        }
    }

    IEnumerator ShowPopup(GameObject popup)
    {
        popup.SetActive(true);
        yield return new WaitForSeconds(3f);
        popup.SetActive(false);
        store.SetActive(true);
    }

    void OnGUI()
    {
        if (ready)
        {
            GUI.skin = mySkin;

            if (GUI.Button(new Rect(Screen.width * (0.3f), Screen.height * (0.6f), Screen.width * (0.15f), Screen.height * (0.1f)), answers[0].ToString()))
            {
                Answer(answers[0]);
            }

            if (GUI.Button(new Rect(Screen.width * (0.45f), Screen.height * (0.6f), Screen.width * (0.15f), Screen.height * (0.1f)), answers[1].ToString()))
            {
                Answer(answers[1]);
            }

            if (GUI.Button(new Rect(Screen.width * (0.60f), Screen.height * (0.6f), Screen.width * (0.15f), Screen.height * (0.1f)), answers[2].ToString()))
            {
                Answer(answers[2]);
            }

            if (GUI.Button(new Rect(Screen.width * (0.75f), Screen.height * (0.6f), Screen.width * (0.15f), Screen.height * (0.1f)), answers[3].ToString()))
            {
                Answer(answers[3]);
            }

            if (GUI.Button(new Rect(Screen.width * (0.3f), Screen.height * (0.8f), Screen.width * (0.15f), Screen.height * (0.1f)), answers[4].ToString()))
            {
                Answer(answers[4]);
            }

            if (GUI.Button(new Rect(Screen.width * (0.45f), Screen.height * (0.8f), Screen.width * (0.15f), Screen.height * (0.1f)), answers[5].ToString()))
            {
                Answer(answers[5]);
            }

            if (GUI.Button(new Rect(Screen.width * (0.60f), Screen.height * (0.8f), Screen.width * (0.15f), Screen.height * (0.1f)), answers[6].ToString()))
            {
                Answer(answers[6]);
            }

            if (GUI.Button(new Rect(Screen.width * (0.75f), Screen.height * (0.8f), Screen.width * (0.15f), Screen.height * (0.1f)), answers[7].ToString()))
            {
                Answer(answers[7]);
            }
        }
    }

    void Answer(int value)
    {
        lockPrompt.SetActive(false);   

        if (value == correctAnswer)
        {
            store.SetActive(true);
            lilyBucksStore.SetActive(true);

            for (int j = 0; j < otherMenus.Length; j++)
            {
                otherMenus[j].SetActive(false);
            }            
        }
        else
        {
            attempts++;
            StartCoroutine(ShowPopup(failPrompt));
        }

        ready = false;
    }
}
