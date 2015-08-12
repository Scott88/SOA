using UnityEngine;
using System.Collections;
using System;
using System.Globalization;

using UnityEngine.Advertisements;

public class AdPlayer : MonoBehaviour
{
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;
    public float minAdGap = 120f;

    public int maxAdsPerSession = 5;

    public bool initialized { get; set; }

    private static bool created = false;

    private int adsShown;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        initialized = false;

        if (Advertisement.isSupported)
        {
            Advertisement.allowPrecache = true;
#if UNITY_ANDROID
#if UNITY_EDITOR
            //Advertisement.Initialize("24667", true);
            //initialized = true;
#else
            //Advertisement.Initialize("24667");
            //initialized = true;
#endif
#elif UNITY_IOS
#if UNITY_EDITOR
			Advertisement.Initialize("31945", true);
            initialized = true;
#else
			Advertisement.Initialize("31945");
            initialized = true;
#endif
#endif
        }
    }

    public void TryShowAd()
    {
        StartCoroutine(ShowAd());
    }

    IEnumerator ShowAd()
    {
        if (Advertisement.isSupported && PassedMinWatchTime() && adsShown <= maxAdsPerSession && SaveFile.Instance().ShowAds())
        {
            yield return new WaitForSeconds(minIdleTime);

            float timer = maxIdleTime - minIdleTime;

            if (timer > 0f && !Advertisement.isReady("defaultVideoAndPictureZone"))
            {
                timer -= Time.deltaTime;
                yield return 0;
            }

            if (Advertisement.isReady("defaultVideoAndPictureZone"))
            {
                ShowOptions options = new ShowOptions();

                options.pause = false;
                options.resultCallback = AdResultCallback;

                Advertisement.Show("defaultVideoAndPictureZone", options);
            }
            else
            {
                FindObjectOfType<LevelLoader>().Load();
            }
        }
        else
        {
            FindObjectOfType<LevelLoader>().Load();
        }
    }

    void AdResultCallback(ShowResult result)
    {
        SaveWatchTime();
        adsShown++;

        FindObjectOfType<LevelLoader>().Load();
    }

    public void TryShowRewardAd()
    {
        StartCoroutine(ShowRewardAd());
    }

    IEnumerator ShowRewardAd()
    {
        if (Advertisement.isSupported && PassedMinWatchTime() && adsShown <= maxAdsPerSession)
        {
            yield return new WaitForSeconds(minIdleTime);

            if (!Advertisement.isReady("rewardedVideoZone"))
            {
                yield return 0;
            }

            ShowOptions options = new ShowOptions();

            options.pause = false;
            options.resultCallback = RewardAdResultCallback;

            Advertisement.Show("rewardedVideoZone", options);
        }
        else
        {
            FindObjectOfType<AdReward>().TooSoon();
        }
    }

    void RewardAdResultCallback(ShowResult result)
    {
        SaveWatchTime();

        if (result == ShowResult.Finished)
        {
            FindObjectOfType<AdReward>().Reward();
        }
    }

    void SaveWatchTime()
    {
        PlayerPrefs.SetString("adTime", DateTime.Now.ToString("o"));
    }

    bool PassedMinWatchTime()
    {
        if (!PlayerPrefs.HasKey("adTime"))
        {
            return true;
        }

        DateTime currentTime = DateTime.Now;
        DateTime adTime = DateTime.Parse(PlayerPrefs.GetString("adTime"), null, DateTimeStyles.RoundtripKind);

        float difference = (float)(currentTime - adTime).TotalSeconds;

        return difference > minAdGap;
    }
}
