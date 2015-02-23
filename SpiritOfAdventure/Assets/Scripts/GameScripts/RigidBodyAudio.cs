using UnityEngine;
using System.Collections;

public class RigidBodyAudio : MonoBehaviour
{
	public AudioSource rollingSource, impactSource;
	public float minCollisionSpeed, maxCollisionSpeed;
	[Range(0f, 1f)]
	public float minCollisionVolume;
	public float minRotationSpeed, maxRotationSpeed;

	private bool madeContact = false;

	void OnCollisionEnter2D(Collision2D coll)
	{
		madeContact = true;

        if (impactSource)
        {
            float magnitude = coll.relativeVelocity.magnitude;
            float volume = 0;

            if (magnitude < minCollisionSpeed)
            {
                volume = 0f;
            }
            else if (magnitude > maxCollisionSpeed)
            {
                volume = 1f;
            }
            else
            {
                volume = ((magnitude - minCollisionSpeed) / (maxCollisionSpeed - minCollisionSpeed)) *
                            (1 - minCollisionVolume) + minCollisionVolume;
            }

            if (!impactSource.isPlaying || volume > 0.1f)
            {
                impactSource.volume = volume;
                impactSource.Play();
            }
        }
	}

	void OnCollisionExit2D(Collision2D coll)
	{
		madeContact = false;

		rollingSource.volume = 0;
	}

	void FixedUpdate()
	{
		float speed = Mathf.Abs(rigidbody2D.angularVelocity);

		if (speed < minRotationSpeed || !madeContact)
		{
			rollingSource.volume = 0f;
		}
		else if (speed > maxRotationSpeed)
		{
			rollingSource.volume = 1f;
		}
		else
		{
			rollingSource.volume = (speed - minRotationSpeed) / (maxRotationSpeed - minRotationSpeed);
		} 
	}
}
