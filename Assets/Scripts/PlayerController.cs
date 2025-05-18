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
    public float launchForce = 5f;
    public float snapDistance = 0.5f;
    public float returnDelay = 3f;

    [Header("Planet Splash Animation")]
    public float startScaleMul = 2f;
    public float endScaleMul = 4f;
    public float planetSplashTime = 0.5f;

    [Header("Death")]
    public float colorBurstOffset = 0.5f;
    [Range(0, 1)] public float colorBurstAlpha = 1.0f;

    [LayoutStart("Info", ELayout.FoldoutBox)]
    [ReadOnly] public uint planetJumpsCounter = 0;
    [ReadOnly] public Transform previousPlanet;
    [LayoutEnd(".")]

    private bool isAttached = true;
    private bool isLaunched = false;
    private Rigidbody2D rb;
    private float launchTime;

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
    }

    void Update()
    {
        if (isAttached && launchAction.triggered && useAction)
        {
            Launch();
        }

        if (isLaunched && Time.time - launchTime >= returnDelay)
        {
            ReturnToPlanet(transform.position, Vector3.right, transform, BurstAnimType.Time);
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

            ValidatePlanetColor(curr, previousPlanet);
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

    private bool ValidatePlanetColor(Planet planet, Transform previousPlanetTransform)
    {
        Transform planetTrans = planet.GetComponent<Transform>();
        if (!planet.CheckIsCorrectColor(CalculatePlayerPlanetAngle(planetTrans), color))
        {
            currentPlanet = previousPlanetTransform;
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
        rb.linearVelocity = direction * launchForce;

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

    void ReturnToPlanet(Vector3 hitPos, Vector3 hitNormal, Transform previousPlanet, BurstAnimType anim = BurstAnimType.Planet)
    {
        Vector2 vel = rb.linearVelocity;
        rb.linearVelocity = Vector2.zero;

        Color primary = ColorsManager.Instance.GetPrimaryColor(color);
        primary.a = colorBurstAlpha;
        Color secondary = ColorsManager.Instance.GetSecondaryColor(color);
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
                        previousPlanet.transform.position,
                        Mathf.Max(previousPlanet.transform.lossyScale.x, previousPlanet.transform.lossyScale.x) * 0.5f
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
        transform.position = currentPlanet.position + (transform.position - currentPlanet.position).normalized * currentPlanet.GetComponent<Planet>().playerRadius;
        AttachToPlanet(currentPlanet);
    }

    void ChangePlanet(Transform planet)
    {
        SplashSpawner.Instance.PlanetSplash(
            planet.transform.position,
            planet.gameObject.GetComponent<Planet>().GetColor(CalculatePlayerPlanetAngle(planet), true),
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

        rb.linearVelocity = Vector2.zero;
        Planet oldCurrPlanetComp = currentPlanet.GetComponent<Planet>();
        if (!oldCurrPlanetComp.IsMulticolor() && !oldCurrPlanetComp.IsColorChanging())
        {
            previousPlanet = currentPlanet;
        }
        currentPlanet = planet;
        isAttached = true;
        isLaunched = false;

        rotateClockwise = planet.GetComponent<PlanetRotation>().rotateClockwise;

        // Ustawienie rotacji przy przyczepieniu
        Vector3 directionToPlanet = (transform.position - currentPlanet.position).normalized;
        float angle = Mathf.Atan2(directionToPlanet.y, directionToPlanet.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.position = currentPlanet.position + (transform.position - currentPlanet.position).normalized * currentPlanet.GetComponent<Planet>().playerRadius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isLaunched && collision.CompareTag("Planet"))
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
                else if (!ValidatePlanetColor(planet, currentPlanet)) return;
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
                pickup.Pickup();

                if (!currentPlanet.GetComponent<Planet>().IsMulticolor() && !currentPlanet.GetComponent<Planet>().IsColorChanging())
                {
                    currentPlanet.GetComponent<Planet>().ChangePlanetColor(pickup.color);
                }
                else
                {
                    previousPlanet.GetComponent<Planet>().ChangePlanetColor(pickup.color);
                    currentPlanet = previousPlanet;
                    previousPlanet = null;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isLaunched && collision.CompareTag("Boundries"))
        {
            ReturnToPlanet(transform.position, Vector3.right, transform, BurstAnimType.Edge);
        }
    }

    public void ChangePlayerColor(ColorType newColor)
    {
        color = newColor;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = ColorsManager.Instance.GetPrimaryColor(color);
        sr.material.SetColor("_Color", ColorsManager.Instance.GetSecondaryColor(color) * 0.5f);
    }
}