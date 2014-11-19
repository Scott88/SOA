using UnityEngine;
using System.Collections;

public class EffectManager : MonoBehaviour
{
	public GameObject greenAcceptedLarge;
	public GameObject blueAcceptedLarge;
	public GameObject redAcceptedLarge;

	public GameObject greenAcceptedSmall;
	public GameObject blueAcceptedSmall;
	public GameObject redAcceptedSmall;

	public GameObject greenFizzle;
	public GameObject blueFizzle;
	public GameObject redFizzle;

	public AudioClip[] buttonSounds;

	private Player player;

	void Awake()
	{
		player = FindObjectOfType<Player>() as Player;
	}

	public void PlaySuccessParticle(SpiritType type, bool useLargeParticle, SpiritReceiver receiver)
	{
		GameObject tempParticle = null;
		
		if (useLargeParticle)
		{
			if (type == SpiritType.ST_GREEN)
			{
				tempParticle = (GameObject)Instantiate(greenAcceptedLarge, receiver.transform.position, Quaternion.identity);
			}
			else if (type == SpiritType.ST_BLUE)
			{
				tempParticle = (GameObject)Instantiate(blueAcceptedLarge, player.transform.position, Quaternion.identity);
				tempParticle.transform.parent = player.transform;
				tempParticle.transform.localPosition = new Vector3(5f, 10f, 0f);
			}
			else if (type == SpiritType.ST_RED)
			{
				tempParticle = (GameObject)Instantiate(redAcceptedLarge, receiver.transform.position, Quaternion.identity);
			}
		}
		else
		{
			if (type == SpiritType.ST_GREEN)
			{
				tempParticle = (GameObject)Instantiate(greenAcceptedSmall, receiver.transform.position, Quaternion.identity);
			}
			else if (type == SpiritType.ST_BLUE)
			{
				tempParticle = (GameObject)Instantiate(blueAcceptedSmall, receiver.transform.position, Quaternion.identity);
				tempParticle.transform.parent = receiver.transform;
			}
			else if (type == SpiritType.ST_RED)
			{
				tempParticle = (GameObject)Instantiate(redAcceptedSmall, receiver.transform.position, Quaternion.identity);
			}
		}

		tempParticle.transform.Translate(new Vector3(0f, 0f, -5f));
		tempParticle.particleSystem.Play();

		Destroy(tempParticle, 10f);
	}

	public void PlayFizzleParticle(SpiritType type, Vector3 position)
	{
		GameObject tempParticle = null;

		if (type == SpiritType.ST_GREEN)
		{
			tempParticle = (GameObject)Instantiate(greenFizzle, position, Quaternion.identity);
		}
		else if (type == SpiritType.ST_BLUE)
		{
			tempParticle = (GameObject)Instantiate(blueFizzle, position, Quaternion.identity);		
		}
		else if (type == SpiritType.ST_RED)
		{
			tempParticle = (GameObject)Instantiate(redFizzle, position, Quaternion.identity);
		}

		tempParticle.transform.Translate(new Vector3(0f, 0f, -5f));
		tempParticle.particleSystem.Play();

		Destroy(tempParticle, 10f);
	}

	public void PlayButtonSound()
	{
		audio.PlayOneShot(buttonSounds[Random.Range(0, buttonSounds.Length - 1)]);
	}
}
