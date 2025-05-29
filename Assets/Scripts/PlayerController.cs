using System.Collections.Generic;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Collider2D))]
[RequireComponent (typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    private enum BurstAnimType
    {
        Planet = 0,
        Edge = 1,
        Time = 2
    }


    [Header("General Settings")]
    public bool useAction = true;
    public InputAction launchAction;
    public ColorType color = ColorType.Red;

    [Header("Planets")]
    public CameraPlanetSnap planetSnap;
    public Transform currentPlanet;

    private bool rotateClockwise;

    [Header("Jump")]
    public AnimationCurve launchSpeedCurve;
    public float launchForce = 5f;
    public float snapDistance = 0.5f;
    public float returnDelay = 2f;
    public TrailRenderer trail;

    [Header("Planet Splash Animation")]
    public float startScaleMul = 2f;
    public float endScaleMul = 4f;
    public float planetSplashTime = 0.5f;

    [Header("Death")]
    public float colorBurstOffset = 0.5f;
    [Range(0, 1)] public float colorBurstAlpha = 1.0f;

    [LayoutStart("Info", ELayout.FoldoutBox)]
    [ReadOnly] public uint planetJumpsCounter = 0;
    //[ReadOnly] public Transform previousPlanet;
    [LayoutEnd(".")]

    private bool isAttached = true;
    private bool isLaunched = false;
    private Rigidbody2D rb;
    private float launchTime;
    private Vector2 launchVel;

    [System.Serializable]
    struct JumpData
    {
        // planet
        public Transform planet;
        public float planetAngle;

        // pickups
        public ColorType startColor;
        public List<ColorChangePickup> pickups;
    }

    private JumpData? currentJump = null;
    private readonly Stack<JumpData> jumpsHistory = new();

    void OnEnable()
    {
        launchAction.Enable();
    }

    void OnDisable()
    {
        launchAction.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        AttachToPlanet(currentPlanet);
        ChangePlayerColor(color);
        trail.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isAttached && launchAction.triggered && useAction)
        {
            Launch();
        }

        if (isLaunched)
        {
            float time = Time.time - launchTime;
            rb.linearVelocity = Vector2.Lerp(launchVel, Vector2.zero, launchSpeedCurve.Evaluate(time.Remap(0.0f, returnDelay, 0.0f, 1.0f)));

            if (time >= returnDelay) {
                ReturnToPlanet(transform.position, Vector3.right, transform, BurstAnimType.Time);
            }
        }
    }

    void FixedUpdate()
    {
        Planet curr = currentPlanet.GetComponent<Planet>();
        if (isAttached && !curr.isEnd)
        {
            transform.position = currentPlanet.position + (transform.position - currentPlanet.position).normalized * curr.playerRadius;
            transform.RotateAround(currentPlanet.position, Vector3.forward, 
                currentPlanet.GetComponent<PlanetRotation>().rotationSpeed * Time.fixedDeltaTime * (rotateClockwise ? -1 : 1));

            ValidatePlanetColor(curr);
        }
    }

    private float CalculatePlayerPlanetAngle(Transform planet)
    {
        Vector3 directionToPlanet = (transform.position - planet.position).normalized;
        float angle = Mathf.Atan2(directionToPlanet.y, directionToPlanet.x) * Mathf.Rad2Deg - 90f;
        Quaternion q = Quaternion.Euler(0f, 0f, angle);
        angle = q.eulerAngles.z;
        return angle + 90.0f; // +90 for base base rotation (player sprite is facing upwards)
    }

    private float CalculateTangentAngle(Vector3 point, Vector3 circleCenter, float radius)
    {
        Vector3 d = point - circleCenter;
        if (d.sqrMagnitude <= radius * radius)
        {
            return 0;
        }
        Vector3 dir = (-d).normalized;
        float beta = Mathf.Asin(radius / d.magnitude) * Mathf.Rad2Deg;
        return 90 - beta;
    }

    private bool ValidatePlanetColor(Planet planet)
    {
        Transform planetTrans = planet.GetComponent<Transform>();
        if (!planet.CheckIsCorrectColor(CalculatePlayerPlanetAngle(planetTrans), color))
        {
            Vector3 normal = (planetTrans.position - transform.position).normalized;
            Vector3 pos = planetTrans.position + (planet.playerRadius + colorBurstOffset) * -normal;
            ReturnToPlanet(pos, normal, planetTrans, BurstAnimType.Planet);
            return false;
        }

        return true;
    }

    public void Launch()
    {
        if (!isAttached) return;

        isAttached = false;
        isLaunched = true;
        launchTime = Time.time;

        Vector2 direction = (transform.position - currentPlanet.position).normalized;
        launchVel = direction * launchForce;
        rb.linearVelocity = launchVel;

        currentJump = new JumpData { 
            planet = currentPlanet, 
            planetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg - currentPlanet.transform.rotation.z,
            startColor = color,
            pickups = new()
        };

        trail.gameObject.SetActive(true);

        ++planetJumpsCounter;
    }

    public void RotateClockwise()
    {
        if (!isAttached) return;

        rotateClockwise = true;
    }

    public void RotateCounterClockwise()
    {
        if (!isAttached) return;

        rotateClockwise = false;
    }

    private void RevertJumpData(JumpData data)
    {
        ChangePlayerColor(data.startColor);

        foreach (var pickup in data.pickups)
        {
            pickup.gameObject.SetActive(true);
            pickup.StartAnimation();
        }

        float angle = data.planetAngle + data.planet.rotation.z;
        Vector3 dir = new(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
        transform.position = data.planet.position + dir * data.planet.GetComponent<Planet>().playerRadius;
        // TODO: obliczanie pozycji wzglêdem planety (k¹t)
        AttachToPlanet(data.planet);
    }

    private void RevertJump()
    {
        isAttached = false;
        if (currentJump != null)
        {
            RevertJumpData(currentJump ?? new JumpData());
        }
        else
        {
            RevertJumpData(jumpsHistory.Pop());
        }
    }

    void ReturnToPlanet(Vector3 hitPos, Vector3 hitNormal, Transform deadlyPlanet, BurstAnimType anim = BurstAnimType.Planet)
    {
        Vector2 vel = launchVel;
        rb.linearVelocity = Vector2.zero;

        Color primary = ColorsManager.Instance.GetColor(color, ColorCategory.Primary);
        primary.a = colorBurstAlpha;
        Color secondary = ColorsManager.Instance.GetColor(color, ColorCategory.Secondary);
        secondary.a = colorBurstAlpha;

        switch (anim)
        {
            case BurstAnimType.Planet:
            {
                SplashSpawner.Instance.ColorHitBurst(
                    primary,
                    secondary,
                    hitPos,
                    hitNormal,
                    vel.normalized,
                    CalculateTangentAngle(
                        hitPos,
                        deadlyPlanet.transform.position,
                        Mathf.Max(deadlyPlanet.transform.lossyScale.x, deadlyPlanet.transform.lossyScale.x) * 0.5f
                    ) - 2f // -2f to avoid lines generated on object
                );
                break;
            }
            case BurstAnimType.Edge:
            case BurstAnimType.Time:
            {
                SplashSpawner.Instance.ColorHitBurst(
                    primary,
                    secondary,
                    hitPos,
                    vel.normalized,
                    vel.normalized,
                    90f,
                    true
                );
                break;
            }
        }
        RevertJump();
    }

    void ChangePlanet(Transform planet)
    {
        SplashSpawner.Instance.PlanetSplash(
           planet.transform.position,
           planet.GetComponent<Planet>().isEnd ? ColorsManager.Instance.GetColor(color, ColorCategory.Secondary) : planet.gameObject.GetComponent<Planet>().GetColor(CalculatePlayerPlanetAngle(planet), true),
           planet.transform.localScale * startScaleMul,
           planet.transform.localScale * endScaleMul,
           planetSplashTime
           //0.55f,
           //0.4f
        );
        AttachToPlanet(planet);
    }

    void AttachToPlanet(Transform planet)
    {
        if (planet != currentPlanet)
        {
            // Create line
            LinesManager.Instance.AddLine(currentPlanet, planet);
        }

        if (currentJump != null)
        {
            jumpsHistory.Push(currentJump ?? new JumpData());
            currentJump = null;
        }

        rb.linearVelocity = Vector2.zero;
        currentPlanet = planet;
        isAttached = true;
        isLaunched = false;
        trail.gameObject.SetActive(false);

        rotateClockwise = planet.GetComponent<PlanetRotation>().rotateClockwise;

        // Ustawienie rotacji przy przyczepieniu
        Vector3 directionToPlanet = (transform.position - currentPlanet.position).normalized;
        float angle = Mathf.Atan2(directionToPlanet.y, directionToPlanet.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.position = currentPlanet.position + (transform.position - currentPlanet.position).normalized * currentPlanet.GetComponent<Planet>().playerRadius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLaunched) return;

        if (collision.CompareTag("Planet"))
        {
            Planet planet = collision.GetComponent<Planet>();
            if (planet.isEnd)
            {
                planetSnap.end = true;
                ChangePlanet(collision.transform);
                LevelManager.Instance.FinishLevel();
            }
            else
            {
                Vector3 normal = (planet.transform.position - transform.position).normalized;
                Vector3 pos = planet.transform.position + (planet.playerRadius + colorBurstOffset) * -normal;
                if (planet.isDeadly) ReturnToPlanet(pos, normal, planet.transform, BurstAnimType.Planet);
                else if (!ValidatePlanetColor(planet)) return;
                else
                {
                    ChangePlanet(collision.transform);
                }
            }
        }
        else if (collision.CompareTag("ColorChange"))
        {
            ColorChangePickup pickup = collision.GetComponent<ColorChangePickup>();
            if (pickup.color != color)
            {
                ChangePlayerColor(pickup.color);
                //pickup.Pickup();
                pickup.StopAnimation();
                pickup.gameObject.SetActive(false);
                currentJump?.pickups.Add(pickup);
            }
        }
    }

    // Obsolete
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (isLaunched && collision.CompareTag("Boundries"))
    //    {
    //        ReturnToPlanet(transform.position, Vector3.right, transform, BurstAnimType.Edge);
    //    }
    //}

    public void ChangePlayerColor(ColorType newColor)
    {
        color = newColor;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = ColorsManager.Instance.GetColor(color, ColorCategory.Primary);
        trail.startColor = ColorsManager.Instance.GetColor(color, ColorCategory.Secondary);
        trail.endColor = ColorsManager.Instance.GetColor(color, ColorCategory.Primary);
        //sr.material.SetColor("_Color", ColorsManager.Instance.GetColor(color, ColorCategory.Secondary) * 0.5f);
    }
}