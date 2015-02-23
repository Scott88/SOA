using UnityEngine;
using System.Collections;

public class CameraMan : MonoBehaviour
{
	public float timeToReachTarget = 1f;
	public float leadingDistance = 10f;
	public float maximumSpeed = 20f;

	private GameObject targetObject = null;
	private Vector3 targetPosition = new Vector3();
	private bool followingObject = false;
	private Vector3 currentVelocity = new Vector3();

    private float targetZoom = 0f;
    private float zoomVel = 0f;

	public bool useManualLead { get; set; }
	private Vector3 manualDirection;

	private float shakeIntensity;
	private float shakeDecay;

	private Vector3 shakeOffset;

    private float shakeMoveTimer = 0.0f;

	public void FollowObject(GameObject target)
	{
		targetObject = target;
		followingObject = true;
	}

	public void SetManualDirection(Vector3 direction)
	{
		manualDirection = direction.normalized;
	}

	public void FollowPosition(Vector3 target)
	{
		targetPosition = target;
		followingObject = false;
	}

    public void FollowWaypoint(CameraWaypoint waypoint)
    {
        FollowPosition(waypoint.transform.position);
        ZoomTo(waypoint.zoom);
    }

    public void StopFollowing()
    {
        followingObject = false;
    }

    public void ZoomTo(float zoom)
    {
        targetZoom = zoom;
    }

	void FixedUpdate()
	{
        if (followingObject && !targetObject)
        {
            followingObject = false;
        }

		if (followingObject)
		{	
			if (targetObject.rigidbody2D)
			{
				Vector3 offset;

				if (!useManualLead)
				{
					Vector3 leadingVec = (Vector3)(targetObject.rigidbody2D.velocity);
					Vector3 targetsVel = new Vector3();

					if (leadingVec.magnitude < 1f)
					{
						targetsVel = new Vector3();
					}
					else
					{
						targetsVel = leadingVec.normalized;
					}

					offset = targetsVel * leadingDistance;
				}
				else
				{
					offset = manualDirection * leadingDistance;
				}

				targetPosition = targetObject.transform.position + offset;
			}
			else
			{
                targetPosition = targetObject.transform.position;
			}
		}

        targetPosition.z = -60f;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, timeToReachTarget, maximumSpeed, Time.fixedDeltaTime);

        if (Time.timeScale >= 1f || shakeMoveTimer >= 1f)
        {
            GetShake();
        }
        else
        {
            shakeMoveTimer += Time.timeScale;
        }

		transform.position += shakeOffset;

        if (camera.orthographicSize != targetZoom && targetZoom != 0)
        {
            camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, targetZoom, ref zoomVel, 0.5f);
        }
	}

	public void ShakeCamera(float intensity, float decayRate)
	{
		shakeIntensity = intensity;
		shakeDecay = decayRate;
	}

	void GetShake()
	{
		if (shakeIntensity >= 0f)
		{
			shakeOffset = Random.insideUnitSphere * shakeIntensity;

			shakeIntensity -= shakeDecay * Time.deltaTime;
		}
		else
		{
			shakeOffset = Vector3.zero;
		}
	}
}

