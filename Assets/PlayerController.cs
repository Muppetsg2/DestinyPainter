using UnityEngine;
using UnityEngine.InputSystem;
using static Colors;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Collider2D))]
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
        if (isAttached)
        {
            transform.position = currentPlanet.position + (transform.position - currentPlanet.position).normalized * currentPlanet.GetComponent<Planet>().playerRadius;
            transform.RotateAround(currentPlanet.position, Vector3.forward, 
                currentPlanet.GetComponent<PlanetRotation>().rotationSpeed * Time.fixedDeltaTime * (rotateClockwise ? -1 : 1));

            if (!currentPlanet.GetComponent<Planet>().CheckIsCorrectColor(transform.rotation.eulerAngles.z, color))
            {
                currentPlanet = previousPlanet;
                ReturnToPlanet();
            }
        }
    }

    public void Launch()
    {
        if (!isAttached) return;

        isAttached = false;
        isLaunched = true;
        launchTime = Time.time;

        Vector2 direction = (transform.position - currentPlanet.position).normalized;
        rb.linearVelocity = direction * launchForce;
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
        transform.position = currentPlanet.position + (transform.position - currentPlanet.position).normalized * currentPlanet.GetComponent<Planet>().playerRadius;
        AttachToPlanet(currentPlanet);
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
        if (currentPlanet.gameObject.GetComponent<Planet>().multicolorPlanet == null && currentPlanet.gameObject.GetComponent<Planet>().colorChangingPlanet == null)
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isLaunched && collision.CompareTag("Planet"))
        {
            if (collision.GetComponent<Planet>().isEnd) planetSnap.end = true;
            if (collision.GetComponent<Planet>().isDeadly) ReturnToPlanet();
            else
            {
                AttachToPlanet(collision.transform);
                ++planetJumpsCounter;
            }
        }
    }

    public void ChangePlayerColor(ColorType newColor)
    {
        color = newColor;
        GetComponent<SpriteRenderer>().color = new Color(colorsDict[color].x, colorsDict[color].y, colorsDict[color].z);
    }
}