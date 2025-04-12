using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    public Transform currentPlanet;
    public float launchForce = 5f;
    public float snapDistance = 0.5f;
    public float returnDelay = 3f;
    public float radius = 1.0f;

    private bool isAttached = true;
    private bool isLaunched = false;
    private Rigidbody2D rb;
    private float launchTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        AttachToPlanet(currentPlanet);
    }

    void Update()
    {
        if (isAttached && Input.GetMouseButtonDown(0))
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
            transform.position = currentPlanet.position + (transform.position - currentPlanet.position).normalized * radius;
            transform.RotateAround(currentPlanet.position, Vector3.forward, currentPlanet.GetComponent<PlanetRotation>().rotationSpeed * Time.fixedDeltaTime * (currentPlanet.GetComponent<PlanetRotation>().rotateClockwise ? -1 : 1));
        }
    }

    void Launch()
    {
        isAttached = false;
        isLaunched = true;
        launchTime = Time.time;

        Vector2 direction = (transform.position - currentPlanet.position).normalized;
        rb.linearVelocity = direction * launchForce;
    }

    void ReturnToPlanet()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = currentPlanet.position + (transform.position - currentPlanet.position).normalized * radius;
        AttachToPlanet(currentPlanet);
    }

    void AttachToPlanet(Transform planet)
    {
        rb.linearVelocity = Vector2.zero;
        currentPlanet = planet;
        isAttached = true;
        isLaunched = false;

        // Ustawienie rotacji przy przyczepieniu
        Vector3 directionToPlanet = (transform.position - currentPlanet.position).normalized;
        float angle = Mathf.Atan2(directionToPlanet.y, directionToPlanet.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isLaunched && collision.CompareTag("Planet"))
        {
            if (collision.GetComponent<Planet>().isDeadly) ReturnToPlanet();
            else AttachToPlanet(collision.transform);
        }
    }
}