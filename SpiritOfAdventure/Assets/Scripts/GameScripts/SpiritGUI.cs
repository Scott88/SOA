using UnityEngine;

public class SpiritGUI : MonoBehaviour
{
	public SpiritType type;
	public float cooldownLength = 1f;

	private float cooldownTimer = 0f;
	private Color originalColor;

	void Start()
	{
		originalColor = renderer.material.color;
	}

	void Update()
	{
		if(cooldownTimer > 0f)
		{
			cooldownTimer -= Time.deltaTime;

			if(cooldownTimer < 0f)
			{
				cooldownTimer = 0f;
			}

			originalColor.a = Mathf.Lerp(1f, 0f, cooldownTimer / cooldownLength);

			renderer.material.color = originalColor;

			//Replace this with whatever sort of visual indicator the future icon will have for cooldown.
			//renderer.material.color = Color.Lerp (originalColor, Color.black, cooldownTimer / cooldownLength);
		}
	}

	public void PutOnCooldown()
	{
		cooldownTimer = cooldownLength;
	}

	public bool IsOnCooldown()
	{
		return cooldownTimer != 0f;
	}
}
