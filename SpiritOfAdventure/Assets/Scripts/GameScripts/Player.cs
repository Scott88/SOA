using UnityEngine;

public class Player : MonoBehaviour 
{
	public AudioClip[] footstepSounds;
	public Vector2 movementForce = new Vector2(5f, 0f);
	public float maxSpeed = 5f;
	public float stoppingDampen = 2.5f;

	private static bool isMoving = true;
	public static bool move = true;
	public static bool isAttached = false; 
	
	private static Vector2 movement;
	public static Animator animator;

	public static bool WalkK;
	public static bool Walk;
	public static bool Idle;
	public Material LightMaterial;
	public Material SpriteDiffuse;

	void Start()
	{
		animator = GetComponent<Animator>();

        animator.SetInteger("costume", SaveFile.Instance().GetCurrentCostume());		

		Proceed ();
	}

	void FixedUpdate () 
	{
		if(isMoving && GetComponent<Rigidbody2D>().velocity.x < maxSpeed)
		{
			float multiplier = (maxSpeed - GetComponent<Rigidbody2D>().velocity.x) / maxSpeed;
			GetComponent<Rigidbody2D>().AddForce(movementForce * multiplier);
		}
		else if(!isMoving)
		{
			GetComponent<Rigidbody2D>().AddForce(-GetComponent<Rigidbody2D>().velocity * stoppingDampen);
		}

		if(GetComponent<Rigidbody2D>().velocity.x > 0.5f)
		{
			move = true;
		}

		else
		{
			move = false;
		}
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.name == "MaterialTrigger")
		{
			GetComponent<Renderer>().material = LightMaterial;
		}

		if(other.name == "MaterialTriggerOut")
		{
			GetComponent<Renderer>().material = SpriteDiffuse;
		}

		if (other.name == "BoatStop")
		{
			Player.Proceed();
		}
	}

	void PlayFootstepSound()
	{
		GetComponent<AudioSource>().PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length - 1)]);
	}

	public static void Stop()
	{
		isMoving = false;

			animator.SetBool ("Walk", true);


	}

	public static void Proceed()
	{
		isMoving = true;

			animator.SetBool ("Walk", false);

	}

	public static void VictoryCheer()
	{
		isMoving = false;
		animator.SetBool("WinAnim",false);
	}
}