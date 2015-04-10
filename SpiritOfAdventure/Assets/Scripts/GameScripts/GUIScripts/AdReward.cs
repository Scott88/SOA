using UnityEngine;
using System.Collections;

using SIS;

public class AdReward : MonoBehaviour 
{
    public int lilyBucksReward = 5;

    public GUISkin mySkin;

    public GameObject rewardText;
    public TextMesh lilyBucksText, loadingAdText;

    public bool allowBack { get; set; }

    public static bool viewed = false;

    public float failTimer = 3f;

    void Start()
    {
        //allowBack = true;

        FindObjectOfType<AdPlayer>().TryShowRewardAd();
    }

    void Update()
    {
        failTimer -= Time.deltaTime;

        if (failTimer <= 0f)
        {
            allowBack = true;
        }
    }

    void OnGUI()
    {
        GUI.skin = mySkin;

        if (allowBack)
        {
            if (GUI.Button(new Rect(Screen.width * (0.82f), Screen.height * (0.82f), Screen.width * (0.15f), Screen.height * (0.15f)), "Back"))
            {
                Application.LoadLevel("MainMenu");
            }
        }
    }

    public void Reward()
    {
        loadingAdText.gameObject.SetActive(false);
        rewardText.SetActive(true);

        DBManager.IncreaseFunds("coins", lilyBucksReward);

        lilyBucksText.text = "New Total: " + DBManager.GetFunds("coins");

        allowBack = true;

        viewed = true;
    }

    public void TooSoon()
    {
        loadingAdText.text = "Try again\nlater!";
        allowBack = true;
    }

}
