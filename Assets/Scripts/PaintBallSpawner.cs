using SaintsField;
using SaintsField.Playa;
using Sych.ShareAssets.Runtime;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PaintBallSpawner : MonoBehaviour
{
    public GameObject painBallPrefab;
    public Vector3 paintBallScale = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 splashScale = new Vector3(2f, 2f, 2f);
    public bool curveShooting = true;
    [HideIf(nameof(curveShooting))] public bool fromCameraCenter = true;
    public float timeBetweenBalls = 2f;
    public float timeToWall = 2f;

    public BoxCollider wallCollider;

    public ColorType currentColor = ColorType.Red;
    public Camera mainCamera;
    public LayerMask cameraRenderMask;
    public LayerMask wallLayer;

    public PlayerInput playerInput;
    private InputAction positionAction;

    public bool playAudio = true;

    public bool randomBalls = false;
    [LayoutShowIf(nameof(randomBalls))]
    [LayoutStart("Random Balls Settings", ELayout.FoldoutBox)]
    [ShowIf(nameof(randomBalls))] public float randomBallsDelay = 2f;
    [ShowIf(nameof(randomBalls))] public bool randomBallsInfinite = false;
    [ShowIf(nameof(randomBalls), "!" + nameof(randomBallsInfinite))] public float randomBallsAnimationTime = 5f;
    [LayoutEnd(".")]
    private float delayCounter = 0f;
    private float animationCounter = 1f;
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

#if UNITY_EDITOR
    // Gizmos
    private Vector3? gizmoStart;
    private Vector3? gizmoDir;
#endif

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
        delayCounter = randomBallsDelay;
        if (!randomBallsInfinite) animationCounter = randomBallsAnimationTime;
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

        delayCounter -= Time.deltaTime;

        if (delayCounter <= 0f)
        {
            currentTime -= Time.deltaTime;

            if (!randomBallsInfinite) animationCounter -= Time.deltaTime;

            if (animationCounter > 0f)
            {
                if (currentTime <= 0f)
                {
                    SpawnRandomBall();
                    currentTime = timeBetweenBalls;
                }
            }
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

        Vector3 dir;
        
        if (mainCamera.orthographic)
        {
            dir = mainCamera.transform.forward;
        }
        else
        {
            dir = (worldPos - mainCamera.transform.position).normalized;
        }

#if UNITY_EDITOR
        gizmoStart = worldPos;
        gizmoDir = dir;
#endif

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
        Vector3 pos = Vector3.zero;
        Vector3 velocity = Vector3.zero;
        if (curveShooting)
        {
            pos = transform.position;
            pos.x = Random.Range(transform.position.x - wallCollider.size.x * 0.5f, transform.position.x + wallCollider.size.x * 0.5f);

            velocity = (wallPos - pos) / timeToWall - 0.5f * timeToWall * Physics.gravity;
        }
        else
        {
            if (fromCameraCenter)
            {
                pos = mainCamera.transform.position + mainCamera.transform.forward * (mainCamera.nearClipPlane * 3f + 2.5f * (mainCamera.orthographic ? 0f : 1f));
            }
            else
            {
                pos = new Vector3(wallPos.x, wallPos.y, mainCamera.transform.position.z - mainCamera.nearClipPlane);
            }
            velocity = (wallPos - pos) / timeToWall;
        }

        GameObject ball = Instantiate(painBallPrefab, pos, Quaternion.identity);
        ball.transform.localScale = paintBallScale;
        ball.GetComponent<Rigidbody>().linearVelocity = velocity;
        ball.GetComponent<Rigidbody>().useGravity = curveShooting;
        ball.GetComponent<PaintBall>().SetColor(color);
        ball.GetComponent<PaintBall>().SetScale(splashScale);
        ball.GetComponent<PaintBall>().spawner = this;
        ball.GetComponent<PaintBall>().SetPlayAudio(playAudio);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (gizmoStart.HasValue && gizmoDir.HasValue)
        {
            Gizmos.color = Color.red;
            Vector3 gizmosEnd = gizmoStart.Value + gizmoDir.Value * 10f;
            Gizmos.DrawLine(gizmoStart.Value, gizmosEnd);

            Gizmos.DrawSphere(gizmoStart.Value, 0.1f);
            Gizmos.DrawSphere(gizmosEnd, 0.1f);
        }
    }
#endif
}