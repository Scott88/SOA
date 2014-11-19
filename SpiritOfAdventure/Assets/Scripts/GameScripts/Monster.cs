using UnityEngine;
using System.Collections;

public enum MonsterDir
{
	MD_LEFT,
	MD_NONE,
	MD_RIGHT
}

public class Monster : MonoBehaviour
{
	public Vector2 movementForce = new Vector2(5f, 0f);
	public float maxSpeed = 5f;
	public float fleeSpeed = 20f;
	public float stoppingDampen = 5f;

	private MonsterDir direction = MonsterDir.MD_RIGHT;

	private Animator animator;
	private GameObject player;

	void Awake()
	{
		player = FindObjectOfType<Player>().gameObject;
		animator = GetComponent<Animator>();
	}

	void Update()
	{
		if (!GameWin.HasWon())
		{
			if (!renderer.IsVisibleFrom(Camera.main) && direction == MonsterDir.MD_LEFT)
			{
				Application.LoadLevel(3);
			}
		}
	}

	void FixedUpdate()
	{
		if (direction == MonsterDir.MD_RIGHT && rigidbody2D.velocity.x < maxSpeed)
		{
			float multiplier = (maxSpeed - rigidbody2D.velocity.x) / maxSpeed;
			rigidbody2D.AddForce(movementForce * multiplier);
		}
		else if (direction == MonsterDir.MD_NONE || GameWin.HasWon())
		{
			rigidbody2D.AddForce(-rigidbody2D.velocity * stoppingDampen);		
		}
		else if (direction == MonsterDir.MD_LEFT && rigidbody2D.velocity.x > -fleeSpeed)
		{
			float multiplier = (maxSpeed - rigidbody2D.velocity.x) / maxSpeed;
			rigidbody2D.AddForce(-movementForce * multiplier);
		}
	}

	void AnimationCallback()
	{
		animator.SetBool("playerInRange", false);
		transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		direction = MonsterDir.MD_LEFT;
	}

	void PlayBoopSound()
	{
		audio.Play();
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject == player && direction == MonsterDir.MD_RIGHT)
		{
			direction = MonsterDir.MD_NONE;
			animator.SetBool("playerInRange", true);
		}
	}
}
