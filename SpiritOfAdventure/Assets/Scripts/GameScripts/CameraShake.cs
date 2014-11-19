using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	private bool Shaking;
	public float ShakeDecay;
	public float ShakeIntensity;
	private Vector3 OriginalPos;
	private Quaternion OriginalRot;

	void Start()
	{
		OriginalPos = Camera.main.transform.localPosition;
		OriginalRot = Camera.main.transform.localRotation;

		Shaking = true;
	}


	// Update is called once per frame
	void Update()
	{
		if (ShakeIntensity > 0)
		{
			Camera.main.transform.localPosition = OriginalPos + Random.insideUnitSphere * ShakeIntensity;
			Camera.main.transform.localRotation = new Quaternion(OriginalRot.x + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
												OriginalRot.y + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
												OriginalRot.z + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
												OriginalRot.w + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f);

			ShakeIntensity -= ShakeDecay;
		}
		else if (Shaking)
		{
			Camera.main.transform.localPosition = OriginalPos;
			Camera.main.transform.localRotation = OriginalRot;

			enabled = false;
		}
	}

	void OnDestroy()
	{
		Camera.main.transform.localPosition = OriginalPos;
		Camera.main.transform.localRotation = OriginalRot;
	}

}
