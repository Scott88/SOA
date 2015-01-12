using UnityEngine;
using System.Collections;

public class Spawner : SOAAction
{
	public Vector3 pA;
	public Vector3 pB;
	public Vector3 pC;


	public Transform player;

	public bool attachPlayer;

	private CameraMan cameraMan;
	private bool selectPath= false;
	
	public int pathA;
	public int pathB;
	public int pathC;

	public float time;

	private float speed;

	// Use this for initialization
	void Awake () 
	{
		//player = FindObjectOfType<Player>() as Player;
		cameraMan = FindObjectOfType<CameraMan>() as CameraMan;

		pathA = PlayerPrefs.GetInt ("pa");
		pathB = PlayerPrefs.GetInt ("pb");
		pathC = PlayerPrefs.GetInt ("pc");

		speed = 1 / time;
	}

	protected override void Activate ()
	{
		selectPath = true;
		if (attachPlayer)
		{
			Player.Stop ();
			player.transform.parent = transform;
			Player.isAttached = true;
			//cameraMan.useManualLead = true;
			//cameraMan.SetManualDirection(pA);
		}

	}

	void FixedUpdate()
	{
		if(PlayerPrefs.HasKey("pa"))
		{
			if (selectPath)
			{
				transform.Translate(pA * Time.fixedDeltaTime * speed);
				
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
					selectPath = false;
				}
			}

		}

		if(PlayerPrefs.HasKey("pb"))
		{
			if (selectPath)
			{
				transform.Translate(pB * Time.fixedDeltaTime * speed);
				
				time -= Time.deltaTime;
				
				if (time <= 0f)
				{
					if(attachPlayer)
					{
						player.transform.parent = null;
						Player.Proceed ();
						Player.isAttached = false;
						//cameraMan.useManualLead = false;
					}
					
					ActionComplete();
					selectPath = false;
				}
			}
			
		}

		if(PlayerPrefs.HasKey("pc"))
		{
			if (selectPath)
			{
				transform.Translate(pC * Time.fixedDeltaTime * speed);
				
				time -= Time.deltaTime;
				
				if (time <= 0f)
				{
					if(attachPlayer)
					{
						player.transform.parent = null;
						Player.Proceed ();
						Player.isAttached = false;
						//cameraMan.useManualLead = false;
					}
					
					ActionComplete();
					selectPath = false;
				}
			}
			
		}
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(transform.position, transform.position + pA);
		Gizmos.DrawLine(transform.position, transform.position + pB);
		Gizmos.DrawLine(transform.position, transform.position + pC);
	}


}
