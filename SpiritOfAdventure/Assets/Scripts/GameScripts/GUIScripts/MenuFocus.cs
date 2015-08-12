using UnityEngine;
using System.Collections;

public class MenuFocus : MonoBehaviour
{
	public AudioClip[] buttonSounds;

	private FocusPoint targetMenu;
	private Vector3 actualTarget;
	private Vector3 posRef;
	private float zoomRef;

	void Awake()
	{
		Vector3 pos = transform.position;
		pos.z = -35f;
		transform.position = pos;

		targetMenu = GameObject.Find(LevelQueue.menuContext).GetComponent<FocusPoint>();
		actualTarget = targetMenu.transform.position;
		actualTarget.z = -35f;
		transform.position = actualTarget;

		if(LevelQueue.startZoomed)
		{
			GetComponent<Camera>().orthographicSize = 10f;
		}
		else
		{
			GetComponent<Camera>().orthographicSize = targetMenu.zoom;
			targetMenu.gameObject.SetActive(true);
		}
	}

	void Update()
	{

#if UNITY_EDITOR
		actualTarget = targetMenu.transform.position;
		actualTarget.z = -35f;
#endif
		transform.position = Vector3.SmoothDamp(Camera.main.transform.position, actualTarget, ref posRef, 0.5f, 50f);
		GetComponent<Camera>().orthographicSize = Mathf.SmoothDamp(GetComponent<Camera>().orthographicSize, targetMenu.zoom, ref zoomRef, 0.5f, 30f);

		if (Vector3.Distance(transform.position, actualTarget) < 0.1f && Mathf.Abs(GetComponent<Camera>().orthographicSize - targetMenu.zoom) < 0.1f &&
		    !targetMenu.gameObject.activeSelf)
		{
			targetMenu.gameObject.SetActive(true);
		}
	}

	public void SetTarget(FocusPoint target)
	{
		targetMenu.gameObject.SetActive(false);
		targetMenu = target;

		actualTarget = targetMenu.transform.position;
		actualTarget.z = -35f;
		//transform.position = targetMenu.transform.position;
		//targetMenu.gameObject.SetActive(true);
	}

	public void PlayButtonSound()
	{
		GetComponent<AudioSource>().PlayOneShot(buttonSounds[Random.Range(0, buttonSounds.Length - 1)]);
	}
}