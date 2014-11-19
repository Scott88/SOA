using UnityEngine;
using System.Collections;

public class ScaleToResolution : MonoBehaviour
{
	void Start()
	{
		float originalRes = 16f / 9f;
		float nextRes = (float)(Screen.width) / (float)(Screen.height);

		float ratio = nextRes / originalRes;

		Vector3 tempPos = transform.localPosition;

		tempPos.x += (1 - ratio) * 9f;

		//transform.localScale *= ratio;
		transform.localPosition = tempPos;
	}
}
