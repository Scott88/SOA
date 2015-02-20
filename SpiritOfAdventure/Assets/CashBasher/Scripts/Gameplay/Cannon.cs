using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour
{
    public float minAngle = 0f, maxAngle = 85f;
    public float angleSpeed = 1.0f;
    public float minVelocity = 5f, maxVelocity = 20f;
    public float velocitySpeed = 1.0f;

    public GameObject cannonBall;

    public GameObject cannonWrapper;

    public GameObject ballSpawnPoint;

    public GameObject angle;
    public GameObject power;

    public GameObject cannonPivot;
    public GameObject markerPivot;
    public GameObject powerIndicator;

    public GameObject baseAndWheels;

    public GameObject frontWheel, backWheel;

    public GameObject cannonSmoke;

    public Animator cannonAnimator;
    
    enum CannonState
    {
        CS_IDLE,
        CS_ROTATING,
        CS_ROTATE_CANNON,
        CS_VELOCITY,
        CS_FIRING,
        CS_FIRED,
        CS_LAST_FIRED
    }

    CannonState currentState = CannonState.CS_IDLE;

    private float velocity;

    private float angleRange;
    private float velocityRange;

    private float anglePeriod, velocityPeriod;

    private float timer;

    private Quaternion startRotation;

    private float knockback;

    private float knockbackTimer;

    public float knockbackDuration = 0.25f;
    public float rollForwardDuration = 0.25f;

    private float rollForwardSpeed;

    void Start()
    {
        angleRange = (maxAngle - minAngle);
        //angleAverage = (minAngle + maxAngle) / 2;

        velocityRange = (maxVelocity - minVelocity);
        //velocityAverage = (minVelocity + maxVelocity) / 2;

        anglePeriod = 1.0f / angleSpeed;
        velocityPeriod = 1.0f / velocitySpeed;

        markerPivot.transform.eulerAngles = new Vector3(0f, 0f, minAngle);
        velocity = minVelocity;

        SetPowerIndicator();

        if (transform.localEulerAngles.y > 90)
        {
            Vector3 pos = markerPivot.transform.localPosition;
            pos.z += 2;
            markerPivot.transform.localPosition = pos;

            pos = powerIndicator.transform.localPosition;
            pos.z += 2;
            powerIndicator.transform.localPosition = pos;

            Vector3 scale = baseAndWheels.transform.localScale;
            scale.z *= -1;
            baseAndWheels.transform.localScale = scale;
        }
    }

    void Update()
    {
        if (currentState == CannonState.CS_ROTATING)
        {
            float angle = GetAngle();

            markerPivot.transform.localEulerAngles = new Vector3(0f, 0f, angle);
        }
        else if (currentState == CannonState.CS_ROTATE_CANNON)
        {
            timer += Time.deltaTime;

            cannonPivot.transform.localRotation = Quaternion.Slerp(startRotation, markerPivot.transform.localRotation, timer * 2);

            if (timer >= 0.5f)
            {
                timer = 0f;

                angle.SetActive(false);
                power.SetActive(true);

                velocity = minVelocity;

                SetPowerIndicator();

                power.transform.localRotation = markerPivot.transform.localRotation;

                cannonPivot.transform.localRotation = markerPivot.transform.localRotation;

                currentState = CannonState.CS_VELOCITY;
            }
        }
        else if (currentState == CannonState.CS_VELOCITY)
        {            
            velocity = GetPower();

            SetPowerIndicator();
        }
        else if (currentState == CannonState.CS_FIRED || currentState == CannonState.CS_LAST_FIRED)
        {      
            Vector3 cannonPos = cannonWrapper.transform.localPosition;

            if (knockbackTimer > 0f)
            {
                knockbackTimer -= Time.deltaTime;
                cannonPos.x = -Mathf.Cos(transform.eulerAngles.y) * Mathf.Sin((knockbackTimer + knockbackDuration) / (knockbackDuration * 2) * Mathf.PI) * knockback;
            }
            else if (Mathf.Abs(cannonPos.x) > 0.001f)
            {
                //cannonPos.x = Mathf.Lerp(-Mathf.Cos(transform.eulerAngles.y) * knockback, 0f, ((rollForwardDuration - rollForwardTimer) / rollForwardDuration));
                cannonPos.x = Mathf.SmoothDamp(cannonPos.x, 0f, ref rollForwardSpeed, rollForwardDuration);
            }
            else
            {
                cannonPos.x = 0f;

                if (currentState == CannonState.CS_FIRED)
                {
                    Activate();
                }
            } 

            Vector3 rotation = new Vector3(0f, 0f, -cannonPos.x * Mathf.Rad2Deg);

            frontWheel.transform.localEulerAngles = rotation;
            backWheel.transform.localEulerAngles = rotation;

            cannonWrapper.transform.localPosition = cannonPos;
        }

        if (currentState != CannonState.CS_IDLE && Input.GetMouseButtonDown(0))
        {
            GetClickedOn();
        }
    }

    float GetAngle()
    {
        timer += Time.deltaTime;

        if(timer >= anglePeriod * 2f)
        {
            timer -= anglePeriod * 2f;
        }

        if (timer < anglePeriod)
        {
            return (timer / anglePeriod) * angleRange + minAngle;
        }
        else
        {
            return ((2f * anglePeriod - timer) / anglePeriod) * angleRange + minAngle;
        }
    }

    float GetPower()
    {
        timer += Time.deltaTime;

        if (timer >= velocityPeriod * 2f)
        {
            timer -= velocityPeriod * 2f;
        }

        if (timer < velocityPeriod)
        {
            return (timer / velocityPeriod) * velocityRange + minVelocity;
        }
        else
        {
            return ((2f * velocityPeriod - timer) / velocityPeriod) * velocityRange + minVelocity;
        }
    }

    void SetPowerIndicator()
    {
        Vector3 indPos = powerIndicator.transform.localPosition;

        indPos.x = ((velocity - minVelocity) / velocityRange) * 2.6f - 1.3f; 

        powerIndicator.transform.localPosition = indPos;
    }

    void GetClickedOn()
    {
        if (currentState == CannonState.CS_ROTATING)
        {
            timer = 0f;
            currentState = CannonState.CS_ROTATE_CANNON;

            startRotation = cannonPivot.transform.localRotation;
        }
        else if (currentState == CannonState.CS_VELOCITY) 
        {
            power.SetActive(false);

            cannonAnimator.SetFloat("Power", (velocity - minVelocity) / velocityRange);
            cannonAnimator.SetTrigger("Fire");

            currentState = CannonState.CS_FIRING;
        }
    }

    public void Fire()
    {
        float angle = cannonPivot.transform.eulerAngles.z * Mathf.Deg2Rad;
        float turnAngle = cannonPivot.transform.eulerAngles.y * Mathf.Deg2Rad;

        Vector3 finalVel = new Vector3(Mathf.Cos(angle) * Mathf.Cos(turnAngle) * velocity, Mathf.Sin(angle) * velocity, 0f);

        Vector3 smokePos = cannonSmoke.transform.localPosition;
        Vector3 spawnPos = ballSpawnPoint.transform.localPosition;

        if ((velocity - minVelocity) / velocityRange >= 0.25f)
        {
            if ((velocity - minVelocity) / velocityRange >= 0.9f)
            {
                knockback = 1f;
            }
            else
            {
                knockback = 0.7f;
            }

            smokePos.x = 1.75f;
            spawnPos.x = 2.45f;
        }
        else
        {
            knockback = 0.5f;

            smokePos.x = 0.8f;
            spawnPos.x = 1.75f;
        }

        knockback = knockback * Mathf.Cos(angle) * Mathf.Cos(turnAngle);

        cannonSmoke.transform.localPosition = smokePos;
        ballSpawnPoint.transform.localPosition = spawnPos;

        GameObject ball = Instantiate(cannonBall, ballSpawnPoint.transform.position, new Quaternion()) as GameObject;

        ball.rigidbody2D.velocity = finalVel;

        audio.Play();

        currentState = CannonState.CS_FIRED;
        knockbackTimer = knockbackDuration;
    }

    public void Activate()
    {
        timer = 0f;
        markerPivot.transform.eulerAngles = new Vector3(0f, 0f, GetAngle());
        currentState = CannonState.CS_ROTATING;

        angle.SetActive(true);
    }

    public void Deactivate()
    {
        if (currentState != CannonState.CS_FIRED)
        {
            currentState = CannonState.CS_IDLE;
        }
        else
        {
            currentState = CannonState.CS_LAST_FIRED;
        }

        power.SetActive(false);
        angle.SetActive(false);
    }
}
