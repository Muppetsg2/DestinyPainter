using Sych.ShareAssets.Runtime;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PaintBallSpawner : MonoBehaviour
{
    public GameObject painBallPrefab;
    public float timeBetweenBalls = 2f;
    public float timeToWall = 2f;

    public BoxCollider wallCollider;

    public ColorType currentColor = ColorType.Red;
    public Camera mainCamera;
    public LayerMask cameraRenderMask;
    public LayerMask wallLayer;

    public PlayerInput playerInput;
    private InputAction positionAction;

    public bool randomBalls = false;
    private float currentTime = 0f;

    public List<RectTransform> canvasButtons = new();

    public RectTransform redBallTransform;
    public RectTransform violetBallTransform;
    public RectTransform blueBallTransform;

    public RectTransform currentBallArrowTransform;

    private List<GameObject> splashes = new();

    private int attemptsCounter = 2;
    private int initAttempts = 2;
    private bool startInit = false;

    void Start()
    {
        Vector3 pos = transform.position;
        pos.y = wallCollider.transform.position.y - wallCollider.size.y * 0.5f;
        transform.position = pos;

        if (mainCamera == null) mainCamera = Camera.main;

        positionAction = playerInput.actions["Point"];

        playerInput.actions["Click"].performed += SpawnPlayerBall;

        startInit = true;
        attemptsCounter = initAttempts;
    }

    void Update()
    {
        if (startInit)
        {
            --attemptsCounter;

            switch (currentColor)
            {
                case ColorType.Red:
                {
                    ChangeToRed();
                    break;
                }
                case ColorType.Violet:
                {
                    ChangeToViolet();
                    break;
                }
                case ColorType.Blue:
                {
                    ChangeToBlue();
                    break;
                }
            }

            if (attemptsCounter == 0)
            {
                startInit = true;
                attemptsCounter = initAttempts;
            }
        }

        if (!randomBalls) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            SpawnRandomBall();
            currentTime = timeBetweenBalls;
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level == SceneManager.GetSceneByName("Paint Balls").buildIndex) return;
        playerInput.actions["Click"].performed -= SpawnPlayerBall;
    }

    private bool IsInCanvasButton(Vector2 screenPos)
    {
        foreach (var canvasBtn in canvasButtons)
        {
            Vector2 localScreenPos = canvasBtn.InverseTransformPoint(screenPos);
            if (canvasBtn.rect.Contains(localScreenPos))
            {
                return true;
            }
        }

        return false;
    }

    public void ShareImage()
    {
        RenderTexture renderTex = new(Screen.width, Screen.height, 0, RenderTextureFormat.DefaultHDR);
        Texture2D texture = new(Screen.width, Screen.height, TextureFormat.RGBAFloat, false);

        LayerMask lastCullingMask = mainCamera.cullingMask;
        mainCamera.cullingMask = cameraRenderMask;
        mainCamera.targetTexture = renderTex;

        mainCamera.Render();

        mainCamera.cullingMask = lastCullingMask;
        mainCamera.targetTexture = null;

        RenderTexture lastTex = RenderTexture.active;
        RenderTexture.active = renderTex;
        texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = lastTex;

        string filePath = Path.Combine(Application.persistentDataPath, "share_image.png");
        File.WriteAllBytes(filePath, texture.EncodeToPNG());

        Destroy(texture);
        Destroy(renderTex);

        Share.Item(filePath, (success) => { Debug.Log("Image: '" + filePath + "' shared"); });
    }

    public void SpawnPlayerBall(InputAction.CallbackContext ctx)
    {
        if (randomBalls) return;
        if (ctx.ReadValue<float>() != 0) return;

        Vector3 position = positionAction.ReadValue<Vector2>();
        position.z = mainCamera.nearClipPlane;

        if (IsInCanvasButton(position)) return;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(position);

        Vector3 dir = (worldPos - mainCamera.transform.position).normalized;

        if (Physics.Raycast(worldPos, dir, out RaycastHit hit, float.MaxValue, wallLayer.value))
        {
            SpawnBall(hit.point, currentColor);
        }
    }

    private void MoveArrowTo(RectTransform ball)
    {
        currentBallArrowTransform.position = ball.position;
        Vector2 pos = currentBallArrowTransform.anchoredPosition;
        pos.y += ball.rect.height * 0.5f + 16f;
        currentBallArrowTransform.anchoredPosition = pos;
    }

    public void ChangeToRed()
    {
        currentColor = ColorType.Red;
        MoveArrowTo(redBallTransform);
    }

    public void ChangeToViolet()
    {
        currentColor = ColorType.Violet;
        MoveArrowTo(violetBallTransform);
    }

    public void ChangeToBlue()
    {
        currentColor = ColorType.Blue;
        MoveArrowTo(blueBallTransform);
    }

    public void BackToMenu()
    {
        LevelLoader.Instance.LoadLevel("Menu");
    }

    public void ResetCanvas()
    {
        while(splashes.Count > 0)
        {
            DestroyImmediate(splashes[0]);
            splashes.RemoveAt(0);
        }
    }

    public void AddSplash(GameObject splash)
    {
        splashes.Add(splash);
    }

    public void RemoveSplash(GameObject splash)
    {
        if (splashes.Remove(splash))
        {
            DestroyImmediate(splash);
        }
    }

    private void SpawnRandomBall()
    {
        Vector3 wallPos = wallCollider.transform.position;
        wallPos.x = Random.Range(wallCollider.transform.position.x - wallCollider.size.x * 0.5f, wallCollider.transform.position.x + wallCollider.size.x * 0.5f);
        wallPos.y = Random.Range(wallCollider.transform.position.y - wallCollider.size.y * 0.5f, wallCollider.transform.position.y + wallCollider.size.y * 0.5f);

        SpawnBall(wallPos, (ColorType)(Random.Range((int)ColorType.None, (int)ColorType.Blue) + 1));
    }

    private void SpawnBall(Vector3 wallPos, ColorType color)
    {
        Vector3 pos = transform.position;
        pos.x = Random.Range(transform.position.x - wallCollider.size.x * 0.5f, transform.position.x + wallCollider.size.x * 0.5f);

        Vector3 velocity = (wallPos - pos) / timeToWall - 0.5f * timeToWall * Physics.gravity;

        GameObject ball = Instantiate(painBallPrefab, pos, Quaternion.identity);
        ball.GetComponent<Rigidbody>().linearVelocity = velocity;
        ball.GetComponent<PaintBall>().SetColor(color);
        ball.GetComponent<PaintBall>().spawner = this;
    }
}
