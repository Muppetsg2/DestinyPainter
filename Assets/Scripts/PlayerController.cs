using UnityEngine;
using UnityEngine.InputSystem;
using static ColorsManager;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Collider2D))]
[RequireComponent (typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    public CameraPlanetSnap planetSnap;
    public Transform currentPlanet;
    public Transform previousPlanet;

    public GameObject linePrefab;

    private bool rotateClockwise;

    public float launchForce = 5f;
    public float snapDistance = 0.5f;
    public float returnDelay = 3f;

    private bool isAttached = true;
    private bool isLaunched = false;
    private Rigidbody2D rb;
    private float launchTime;

    public bool useAction = true;

    public InputAction launchAction;

    public ColorType color = ColorType.Red;

    public uint planetJumpsCounter = 0;

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
            ReturnToPlanet();
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

    private bool ValidatePlanetColor(Planet planet, Transform previousPlanetTransform)
    {
        if (!planet.CheckIsCorrectColor(CalculatePlayerPlanetAngle(planet.GetComponent<Transform>()), color))
        {
            currentPlanet = previousPlanetTransform;
            ReturnToPlanet();
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

    void ReturnToPlanet()
    {
        rb.linearVelocity = Vector2.zero;
        SplashSpawner.Instance.PlayerDeathSplashes(
            transform.position,
            ColorsManager.Instance.GetPrimaryColor(color),
            ColorsManager.Instance.GetSecondaryColor(color)
        );
        transform.position = currentPlanet.position + (transform.position - currentPlanet.position).normalized * currentPlanet.GetComponent<Planet>().playerRadius;
        AttachToPlanet(currentPlanet);
    }

    void ChangePlanet(Transform planet)
    {
        SplashSpawner.Instance.PlanetSplash(
            planet.transform.position,
            planet.gameObject.GetComponent<Planet>().GetColor(CalculatePlayerPlanetAngle(planet), true),
            planet.transform.localScale * 2.0f,
            planet.transform.localScale * 3.0f,
            0.3f,
            0.55f,
            0.4f
        );
        AttachToPlanet(planet);
    }

    void AttachToPlanet(Transform planet)
    {
        if (planet != currentPlanet)
        {
            // Create line
            GameObject line = Instantiate(linePrefab, null, true);
            line.name = currentPlanet.name + " -> " + planet.name;

            line.transform.localScale = new Vector3(Vector3.Distance(planet.position, currentPlanet.position), line.transform.localScale.y, 1f);
            line.transform.localPosition = (planet.position + currentPlanet.position) * 0.5f;

            Vector3 direction = (planet.position - currentPlanet.position).normalized;

            float lineAngle = Vector3.Angle(Vector3.right, direction);
            float lineAngleSign = Mathf.Sign(Vector3.Dot(Vector3.forward, Vector3.Cross(Vector3.right, direction)));

            line.transform.localEulerAngles = new Vector3(0f, 0f, lineAngleSign * lineAngle);
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
                LevelManager.instance.FinishLevel();
            }
            else
            {
                if (planet.isDeadly) ReturnToPlanet();
                else if (!ValidatePlanetColor(planet, currentPlanet)) return;
                else
                {
                    ChangePlanet(collision.transform);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isLaunched && collision.CompareTag("Boundries"))
        {
            ReturnToPlanet();
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