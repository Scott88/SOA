using UnityEngine;
using System.Collections;

public class IntroScreen : MonoBehaviour
{
	public float fadeInTime, displayTime, fadeOutTime;

    public Vector3 translateTo, scaleTo;

    public TextMesh obbText;

    private Vector3 translateVel, scaleVel;

	private Color alphaController;

    private Vector3 startTranslation, startScale;

	void Start()
	{
		translateVel = new Vector3 ();
		scaleVel = new Vector3 ();

		alphaController = renderer.material.color;
		alphaController.a = 0;
		renderer.material.color = alphaController;

        startTranslation = transform.position;
        startScale = transform.localScale;

		StartCoroutine(Go());
		Time.timeScale = 1;
	}

	IEnumerator Go()
	{
		float timer = fadeInTime;

		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			alphaController.a = 1 - timer / fadeInTime;
			renderer.material.color = alphaController;
			yield return 0;
		}

        timer = displayTime;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return 0;
        }

#if !UNITY_EDITOR

        if (!OBBReady())
        {
            if (CheckExpPath())
            {

                while (Mathf.Abs(transform.localScale.x - scaleTo.x) > 0.0001f)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, translateTo, ref translateVel, 0.2f);
                    transform.localScale = Vector3.SmoothDamp(transform.localScale, scaleTo, ref scaleVel, 0.2f);
                    yield return 0;
                }

                timer = 2.0f;
                
                obbText.text = "Downloading game content...";

                while(timer > 0.0f)
                {
                    timer -= Time.deltaTime;
                    yield return 0;
                }       
  
                GetOBB();

                while (!OBBReady())
                {
                    yield return 0;
                }

                obbText.text = "";

                while (Mathf.Abs(transform.localScale.x - startScale.x) > 0.0001f)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, startTranslation, ref translateVel, 0.2f);
                    transform.localScale = Vector3.SmoothDamp(transform.localScale, startScale, ref scaleVel, 0.2f);
                    yield return 0;
                }
            }
            else
            {
                yield break;
            }
        }

#endif           

		timer = fadeOutTime;

		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			alphaController.a = timer / fadeOutTime;
			renderer.material.color = alphaController;
			yield return 0;
		}

		Application.LoadLevel("CutScene");
	}

    bool CheckExpPath()
    {
        if (!GooglePlayDownloader.RunningOnAndroid())
        {
            return true;
        }

        string expPath = GooglePlayDownloader.GetExpansionFilePath();

        if (expPath == null)
        {
            obbText.text = "Not enough room for full game";
            return false;
        }

        return true;
    }

    void GetOBB()
    {
        string mainPath = GooglePlayDownloader.GetMainOBBPath(GooglePlayDownloader.GetExpansionFilePath());

        if (mainPath == null)
        {
            GooglePlayDownloader.FetchOBB();
        }
    }

    bool OBBReady()
    {
        return !GooglePlayDownloader.RunningOnAndroid() || GooglePlayDownloader.GetMainOBBPath(GooglePlayDownloader.GetExpansionFilePath()) != null;
    }
}
