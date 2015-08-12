//~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*
//SpiritReceiver.cs
//
//By: Nicholas MacDonald
//A trigger that triggers when it receives a compatable spirit.
//~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*

using UnityEngine;

public class SpiritReceiver : Triggerable
{
	protected GameManager gameManager;
	public SpiritType compatableType;
	public EffectSize effect = EffectSize.ST_SMALL;



	void Awake()	
	{
		gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;

	}



	//We override Trigger here just so we can have a fancy particle effect play whenever a spirit is accepted
	public override void Trigger(Vector3 position)
	{
		if (GetComponent<ParticleSystem>())
		{
			GetComponent<ParticleSystem>().Play();
		}
		else if (effect == EffectSize.ST_SMALL)
		{
			gameManager.effectManager.PlaySuccessParticle(compatableType, false, this);
		}
		else if (effect == EffectSize.ST_LARGE)
		{
			gameManager.effectManager.PlaySuccessParticle(compatableType, true, this);
		}

		position.z = transform.position.z - 0.01f;

		base.Trigger(position);
	}
	
}

