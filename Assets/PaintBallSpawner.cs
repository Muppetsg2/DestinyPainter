using UnityEngine;

public class PaintBallSpawner : MonoBehaviour
{
    public GameObject painBallPrefab;
    public float timeBetweenBalls = 2f;
    public float timeToWall = 2f;

    public BoxCollider wallCollider;

    private float currentTime = 0f;

    void Start()
    {
        Vector3 pos = transform.position;
        pos.y = wallCollider.transform.position.y - wallCollider.size.y * 0.5f;
        transform.position = pos;
    }

    void Update()
    {
        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            SpawnBall();
            currentTime = timeBetweenBalls;
        }
    }

    void SpawnBall()
    {
        Vector3 pos = transform.position;
        pos.x = Random.Range(transform.position.x - wallCollider.size.x * 0.5f, transform.position.x + wallCollider.size.x * 0.5f);

        Vector3 wallPos = wallCollider.transform.position;
        wallPos.x = Random.Range(wallCollider.transform.position.x - wallCollider.size.x * 0.5f, wallCollider.transform.position.x + wallCollider.size.x * 0.5f);
        wallPos.y = Random.Range(wallCollider.transform.position.y - wallCollider.size.y * 0.5f, wallCollider.transform.position.y + wallCollider.size.y * 0.5f);

        Vector3 velocity = (wallPos - pos) / timeToWall - 0.5f * timeToWall * Physics.gravity;

        GameObject ball = Instantiate(painBallPrefab, pos, Quaternion.identity);
        ball.GetComponent<Rigidbody>().linearVelocity = velocity;
        ball.GetComponent<PaintBall>().SetColor((ColorType)(Random.Range((int)ColorType.None, (int)ColorType.Blue) + 1));
    }
}
