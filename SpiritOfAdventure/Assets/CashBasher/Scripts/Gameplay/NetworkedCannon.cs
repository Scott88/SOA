﻿using UnityEngine;
using System.Collections;

public class NetworkedCannon : MonoBehaviour
{
    public float minAngle = 0f, maxAngle = 85f;
    public float angleSpeed = 1.0f;
    public float minVelocity = 5f, maxVelocity = 20f;
    public float velocitySpeed = 1.0f;

    public float redDebuffSpeed = 3f;
    public float blueDebuffCap = 0.6f;
    public float greenDebuffCap = 0.6f;

    public int team;

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

    public TextMesh indicator;

    public Animator cannonAnimator;

    public Renderer cannonRenderer;


    enum CannonState
    {
        CS_IDLE,
        CS_ROTATING,
        CS_ROTATE_CANNON,
        CS_VELOCITY,
        CS_FIRING,
        CS_FIRED
    }

    CannonState currentState = CannonState.CS_IDLE;

    private bool myCannon;

    private float velocity;

    private float angleRange;
    private float velocityRange;

    private float anglePeriod, velocityPeriod;

    private float timer;

    private Quaternion startRotation;

    private float knockback;

    private float knockbackTimer;

    private float knockbackDuration = 0.25f;
    private float rollForwardDuration = 0.25f;

    private float rollForwardSpeed;

    private CashBasherManager manager;

    private SpiritType buff, debuff;

    private float fadeOutTimer;
    private Color indicatorColor;

    void Start()
    {
        manager = FindObjectOfType<CashBasherManager>();
        myCannon = team == manager.myTeam;
        
        angleRange = (maxAngle - minAngle);
        //angleAverage = (minAngle + maxAngle) / 2;

        velocityRange = (maxVelocity - minVelocity);
        //velocityAverage = (minVelocity + maxVelocity) / 2;

        anglePeriod = 1.0f / angleSpeed;
        velocityPeriod = 1.0f / velocitySpeed;

        markerPivot.transform.eulerAngles = new Vector3(0f, 0f, minAngle);
        velocity = minVelocity;

        SetPowerIndicator();

        indicatorColor = indicator.GetComponent<Renderer>().material.color;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.tag == "CannonBall")
        {
            NetworkedCannonBall ball = coll.gameObject.GetComponent<NetworkedCannonBall>() as NetworkedCannonBall;

            if (!ball.GetComponent<NetworkView>().isMine || myCannon)
            {
                return;
            }

            if (ball.GetEnchantment() != SpiritType.ST_NULL)
            {
                GetComponent<NetworkView>().RPC("ApplyDebuff", RPCMode.All, (int)ball.GetEnchantment());
            }

            ball.Damage();
        }
    }

    void Update()
    {
        if (fadeOutTimer > 0f)
        {
            fadeOutTimer -= Time.deltaTime;

            indicatorColor.a = Mathf.Lerp(0.0f, 1.0f, fadeOutTimer);
            indicator.GetComponent<Renderer>().material.color = indicatorColor;
        }

        if (currentState == CannonState.CS_ROTATING)
        {
            float angle = GetAngle();

            markerPivot.transform.localEulerAngles = new Vector3(0f, 0f, angle);
        }
        else if (currentState == CannonState.CS_ROTATE_CANNON)
        {
            timer += Time.deltaTime;

            cannonPivot.transform.localRotation = Quaternion.Slerp(startRotation, markerPivot.transform.localRotation, timer * 2);

            if (timer >= 0.5f && myCannon)
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
        else if (currentState == CannonState.CS_FIRED)
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
                currentState = CannonState.CS_IDLE;
            }

            Vector3 rotation = new Vector3(0f, 0f, -cannonPos.x * Mathf.Rad2Deg);

            frontWheel.transform.localEulerAngles = rotation;
            backWheel.transform.localEulerAngles = rotation;

            cannonWrapper.transform.localPosition = cannonPos;
        }
    }

    float GetAngle()
    {
        if (debuff == SpiritType.ST_RED)
        {
            timer += Time.deltaTime * redDebuffSpeed;
        }
        else
        {
            timer += Time.deltaTime;
        }

        if (timer >= anglePeriod * 2f)
        {
            timer -= anglePeriod * 2f;
        }

        float timePoint;

        if (timer < anglePeriod)
        {
            timePoint = timer;
        }
        else
        {
            timePoint = 2f * anglePeriod - timer;
        }

        if (debuff == SpiritType.ST_GREEN)
        {
            return (timePoint / anglePeriod) * angleRange * greenDebuffCap + minAngle;
        }
        else
        {
            return (timePoint / anglePeriod) * angleRange + minAngle;
        }
    }

    float GetPower()
    {
        if (debuff == SpiritType.ST_RED)
        {
            timer += Time.deltaTime * redDebuffSpeed;
        }
        else
        {
            timer += Time.deltaTime;
        }

        if (timer >= velocityPeriod * 2f)
        {
            timer -= velocityPeriod * 2f;
        }

        float timePoint;

        if (timer < velocityPeriod)
        {
            timePoint = timer;
        }
        else
        {
            timePoint = 2f * velocityPeriod - timer;
        }

        if (debuff == SpiritType.ST_BLUE)
        {
            return (timePoint / velocityPeriod) * velocityRange * greenDebuffCap + minVelocity;
        }
        else
        {
            return (timePoint / velocityPeriod) * velocityRange + minVelocity;
        }
    }

    void SetPowerIndicator()
    {
        Vector3 indPos = powerIndicator.transform.localPosition;

        indPos.x = ((velocity - minVelocity) / velocityRange) * 2.6f - 1.3f;

        powerIndicator.transform.localPosition = indPos;
    }

    public bool Press()
    {
        if (myCannon)
        {
            if (currentState == CannonState.CS_ROTATING)
            {
                GetComponent<NetworkView>().RPC("PointCannonAt", RPCMode.All, markerPivot.transform.localRotation);

                return false;
            }
            else if (currentState == CannonState.CS_VELOCITY)
            {
                power.SetActive(false);

                GetComponent<NetworkView>().RPC("LightTheFuse", RPCMode.All, velocity);

                return true;
            }
        }

        return false;
    }

    public void ForceFire()
    {
        GetComponent<NetworkView>().RPC("LightTheFuse", RPCMode.All, minVelocity);
    }

    [RPC]
    void PointCannonAt(Quaternion direction)
    {
        timer = 0f;
        currentState = CannonState.CS_ROTATE_CANNON;

        startRotation = cannonPivot.transform.localRotation;

        markerPivot.transform.localRotation = direction;

        StartCoroutine(DisplayMessage(direction.eulerAngles.z.ToString("0") + "°"));
    }

    [RPC]
    void LightTheFuse(float vel)
    {
        power.SetActive(false);
        angle.SetActive(false);

        cannonAnimator.SetFloat("Power", (vel - minVelocity) / velocityRange);
        cannonAnimator.SetTrigger("Fire");

        velocity = vel;

        currentState = CannonState.CS_FIRING;

        StartCoroutine(DisplayMessage(((vel - minVelocity) / velocityRange * 100f).ToString("0") + "%"));
    }

    public bool CanApplyBuff(SpiritType type)
    {
        return debuff == SpiritType.ST_NULL ||
               debuff == SpiritType.ST_GREEN && type == SpiritType.ST_RED ||
                debuff == SpiritType.ST_BLUE && type == SpiritType.ST_GREEN ||
                debuff == SpiritType.ST_RED && type == SpiritType.ST_BLUE;
    }

    public void ApplyBuff(SpiritType type)
    {
        if(debuff == SpiritType.ST_NULL)
        {
            buff = type;
        }
        else if (debuff == SpiritType.ST_GREEN && type == SpiritType.ST_RED ||
                debuff == SpiritType.ST_BLUE && type == SpiritType.ST_GREEN ||
                debuff == SpiritType.ST_RED && type == SpiritType.ST_BLUE)
        {
            debuff = SpiritType.ST_NULL;
            cannonRenderer.material.color = Color.white;
        }
    }

    [RPC]
    void ApplyDebuff(int type)
    {
        debuff = (SpiritType)type;

        Color color = cannonRenderer.material.color;

        if (debuff != SpiritType.ST_GREEN)
        {
            color.g = 0.5f;
        }

        if (debuff != SpiritType.ST_BLUE)
        {
            color.b = 0.5f;
        }

        if (debuff != SpiritType.ST_RED)
        {
            color.r = 0.5f;
        }

        cannonRenderer.material.color = color;
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

        if (myCannon)
        {
			GameObject ball = Network.Instantiate(cannonBall, ballSpawnPoint.transform.position, new Quaternion(), 0) as GameObject;

            ball.GetComponent<Rigidbody2D>().velocity = finalVel;
            ball.GetComponent<NetworkView>().RPC("SetVelocity", RPCMode.Others, finalVel);

            if (buff != SpiritType.ST_NULL)
            {
                ball.GetComponent<NetworkView>().RPC("Enchant", RPCMode.All, (int)buff);
            }
        }

        GetComponent<AudioSource>().Play();

        manager.cameraMan.ShakeCamera(0.75f, 1.0f);

        buff = SpiritType.ST_NULL;
        debuff = SpiritType.ST_NULL;
        cannonRenderer.material.color = Color.white;

        currentState = CannonState.CS_FIRED;
        knockbackTimer = knockbackDuration;
    }

    IEnumerator DisplayMessage(string message)
    {
        indicatorColor.a = 1f;
        indicator.GetComponent<Renderer>().material.color = indicatorColor;

        fadeOutTimer = -1f;

        indicator.text = message;

        float timer = 2.0f;

        while (timer > 0f)
        {
            fadeOutTimer = -1f;

            timer -= Time.deltaTime;
            yield return 0;
        }

        fadeOutTimer = 1f;
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

        power.SetActive(false);
        angle.SetActive(false);
    }
}
