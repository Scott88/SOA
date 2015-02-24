using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
	public GameObject pointFloaterPref;
	public GameObject firstStar, secondStar, thirdStar;
	public ParticleSystem greenSaved, blueSaved, redSaved;
    public ParticleSystem starEarned;
	public int greenFailsAllowed, blueFailsAllowed, redFailsAllowed;
	public int pointsLostPerUse, pointsGainedPerSaved;
	public int pointsPerSecond;
	public int silverScore, goldScore;

	private int levelScore = 0, spiritGainScore = 0, timerScore = 0;
	private int greenFails = 0, blueFails = 0, redFails = 0;
	private float timeLeft, displayTime;
	private bool animatingScore = false;
	private int starCount;

    private int previousStarCount;

	void Start()
	{
		firstStar.SetActive(false);
		secondStar.SetActive(false);
		thirdStar.SetActive(false);

        previousStarCount = SaveFile.Instance().GetLevelStars(Application.loadedLevelName);
	}
	
	void Update()
	{
		guiText.text = GetFinalScore().ToString();

		if ((GameWin.HasWon() || PauseButton.paused) && guiText.enabled)
		{
			guiText.enabled = false;
		}
		else if (!(GameWin.HasWon() || PauseButton.paused) && !guiText.enabled)
		{
			guiText.enabled = true;
		}

		if (animatingScore)
		{
			if (!firstStar.activeSelf)
			{
				firstStar.SetActive(true);
				starCount = 1;

                if (starCount > previousStarCount)
                {
                    starEarned.Emit(1);
                }
			}

			if (!secondStar.activeSelf && GetFinalScore() > silverScore)
			{
				secondStar.SetActive(true);
				starCount = 2;

                if (starCount > previousStarCount)
                {
                    starEarned.Emit(1);
                }
			}

			if (!thirdStar.activeSelf && GetFinalScore() > goldScore)
			{
				thirdStar.SetActive(true);
				starCount = 3;

                if (starCount > previousStarCount)
                {
                    starEarned.Emit(1);
                }
			}
		}
	}

	public void SpiritFailed(SpiritType type)
	{
		if (type == SpiritType.ST_GREEN)
		{
			greenFails++;

			if (greenFails > greenFailsAllowed)
			{
				SpawnFloater(-pointsLostPerUse);
			}
		}
		else if (type == SpiritType.ST_BLUE)
		{
			blueFails++;

			if (blueFails > blueFailsAllowed)
			{
				SpawnFloater(-pointsLostPerUse);
			}
		}
		else if (type == SpiritType.ST_RED)
		{
			redFails++;

			if (redFails > redFailsAllowed)
			{
				SpawnFloater(-pointsLostPerUse);
			}
		}
	}

	public void SpawnFloater(int score)
	{
		PointFloater pointFloater = ((GameObject)Instantiate(pointFloaterPref)).GetComponent<PointFloater>();
		pointFloater.SetPoints(score);
	}

	public void AddScore(int score)
	{
		levelScore += score;

		if (levelScore < 0)
		{
			levelScore = 0;
		}
	}

	public bool IsAnimatingScore()
	{
		return animatingScore;
	}

	public int GetFinalScore()
	{
		return levelScore + spiritGainScore + timerScore;
	}

	public int GetActualFinalScore()
	{
		/*int scoreS = 0;

		int greenScore = greenFailsAllowed - greenFails;
		int blueScore = blueFailsAllowed - blueFails;
		int redScore = redFailsAllowed - redFails;

		if (greenScore > 0)
		{
			scoreS += greenScore * pointsGainedPerSaved;
		}

		if (blueScore > 0)
		{
			scoreS += blueScore * pointsGainedPerSaved;
		}

		if (redScore > 0)
		{
			scoreS += redScore * pointsGainedPerSaved;
		}

		return levelScore + scoreS + (int)timeLeft * pointsPerSecond;*/

		return levelScore + (int)timeLeft * pointsPerSecond;
	}

	public int GetDisplayTimer()
	{
		return (int)displayTime;
	}

	public void CalculateFinalScore(float time)
	{
		timeLeft = displayTime = time;
		StartCoroutine(AnimateFinalScore());
	}

	IEnumerator AnimateFinalScore()
	{
		animatingScore = true;

		//yield return new WaitForSeconds(1);
		//yield return StartCoroutine(AnimateSpiritsSaved());

		yield return new WaitForSeconds(1f);
		yield return StartCoroutine(AnimateTimeSaved());

		if (animatingScore)
		{
            int previousStars = SaveFile.Instance().GetLevelStars(Application.loadedLevelName);

            if (previousStars < starCount)
            {
                SaveFile.Instance().SetLevelStars(Application.loadedLevelName, starCount);

                SaveFile.Instance().ModifyStars(starCount - previousStars);
            }
            else
            {
                starEarned.Emit(1);
                SaveFile.Instance().ModifyStars(1);
            }       

            if (SaveFile.Instance().GetScore(Application.loadedLevelName) < GetActualFinalScore())
            {
                SaveFile.Instance().SetLevelScore(Application.loadedLevelName, GetActualFinalScore());
            }

			animatingScore = false;
		}
	}

	IEnumerator AnimateSpiritsSaved()
	{
		int greenScore = greenFailsAllowed - greenFails;
		int blueScore = blueFailsAllowed - blueFails;
		int redScore = redFailsAllowed - redFails;

		for (int j = 0; j < greenScore; j++)
		{
			spiritGainScore += pointsGainedPerSaved;
			greenSaved.Emit(1);
			yield return new WaitForSeconds(0.5f);
		}

		for (int j = 0; j < blueScore; j++)
		{
			spiritGainScore += pointsGainedPerSaved;
			blueSaved.Emit(1);
			yield return new WaitForSeconds(0.5f);
		}

		for (int j = 0; j < redScore; j++)
		{
			spiritGainScore += pointsGainedPerSaved;
			redSaved.Emit(1);
			yield return new WaitForSeconds(0.5f);
		}	
	}

	IEnumerator AnimateTimeSaved()
	{
		float fullTimeScore = (int)timeLeft * pointsPerSecond;
		float scrollTime = Mathf.Log(timeLeft);
		float time = 0;

		while (time <= scrollTime)
		{
			time += Time.deltaTime;
			timerScore = (int)Mathf.Lerp(0f, (float)fullTimeScore, time / scrollTime);
			displayTime = Mathf.Lerp(timeLeft, 0f, time / scrollTime);

			if (animatingScore)
			{
				audio.Play();
			}

			yield return 0;
		}
	}

	public void SkipAnimation()
	{
		animatingScore = false;

		int finalScore = GetActualFinalScore();

		if (!firstStar.activeSelf)
		{
			firstStar.SetActive(true);
			starCount = 1;
		}

		if (!secondStar.activeSelf && finalScore > silverScore)
		{
			secondStar.SetActive(true);
			starCount = 2;
		}

		if (!thirdStar.activeSelf && finalScore > goldScore)
		{
			thirdStar.SetActive(true);
			starCount = 3;
		}

        int previousStars = SaveFile.Instance().GetLevelStars(Application.loadedLevelName);

        if (previousStars < starCount)
        {
            SaveFile.Instance().SetLevelStars(Application.loadedLevelName, starCount);

            starEarned.Emit(starCount - previousStars);
            SaveFile.Instance().ModifyStars(starCount - previousStars);
        }
        else
        {
            starEarned.Emit(1);
            SaveFile.Instance().ModifyStars(1);
        }

        if (SaveFile.Instance().GetScore(Application.loadedLevelName) < GetActualFinalScore())
        {
            SaveFile.Instance().SetLevelScore(Application.loadedLevelName, GetActualFinalScore());
        }
	}
}
