using UnityEngine;
using System.Collections;

using UnityEngine.Advertisements;

public class AdPlayer : MonoBehaviour
{
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;
    public float minAdGap = 120f;

    public int maxAdsPerSession = 5;

    private static bool created = false;

    private float timeSinceLastAd;

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
        if (Advertisement.isSupported)
        {
            Advertisement.allowPrecache = true;
#if UNITY_EDITOR
            Advertisement.Initialize("24667", true);
#else
            Advertisement.Initialize("24667");
#endif

            timeSinceLastAd = minAdGap;
        }
    }

    void Update()
    {
        timeSinceLastAd += Time.deltaTime;
    }

    public void TryShowAd()
    {
        StartCoroutine(ShowAd());
    }

    IEnumerator ShowAd()
    {
        if (Advertisement.isSupported && timeSinceLastAd > minAdGap && adsShown <= maxAdsPerSession && SaveFile.Instance().ShowAds())
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
        timeSinceLastAd = 0f;
        adsShown++;

        FindObjectOfType<LevelLoader>().Load();
    }

    public void TryShowRewardAd()
    {
        StartCoroutine(ShowRewardAd());
    }

    IEnumerator ShowRewardAd()
    {
        if (Advertisement.isSupported && timeSinceLastAd > minAdGap && adsShown <= maxAdsPerSession)
        {
            yield return new WaitForSeconds(minIdleTime);

            if (!Advertisement.isReady("rewardedVideoZone"))
            {
                yield return 0;
            }

            FindObjectOfType<AdReward>().allowBack = false;

            ShowOptions options = new ShowOptions();

            options.pause = false;
            options.resultCallback = RewardAdResultCallback;

            Advertisement.Show("rewardedVideoZone", options);
        }
    }

    void RewardAdResultCallback(ShowResult result)
    {
        timeSinceLastAd = 0f;

        if (result == ShowResult.Finished)
        {
            FindObjectOfType<AdReward>().Reward();
        }
    }
}
