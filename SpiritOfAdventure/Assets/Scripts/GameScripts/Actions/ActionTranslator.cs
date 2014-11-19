using UnityEngine;
using System.Collections;

public class ActionTranslator : Action
{
	public Vector3 offset;
	public float time;
	public bool attachPlayer;

	private float speed;
	private Player player;
	private CameraMan cameraMan;
	private bool isTranslating = false;

	void Awake()
	{
		player = FindObjectOfType<Player>() as Player;
		cameraMan = FindObjectOfType<CameraMan>() as CameraMan;
	}

	void Start()
	{
		speed = 1 / time;
	}

	protected override void Activate()
	{
		isTranslating = true;
		if (attachPlayer)
		{
			Player.Stop ();
			player.transform.parent = transform;
			Player.isAttached = true;
			cameraMan.useManualLead = true;
			cameraMan.SetManualDirection(offset);
		}
	}

	void FixedUpdate()
	{
		if (isTranslating)
		{
			transform.Translate(offset * Time.fixedDeltaTime * speed);

			time -= Time.deltaTime;

			if (time <= 0f)
			{
				if(attachPlayer)
				{
					player.transform.parent = null;
					Player.Proceed ();
					Player.isAttached = false;
					cameraMan.useManualLead = false;
				}

				ActionComplete();
				isTranslating = false;
			}
		}
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(transform.position, transform.position + offset);
	}
}
