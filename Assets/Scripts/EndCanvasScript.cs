using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndCanvasScript : MonoBehaviour
{
    public PlayerController player;
    public Camera mainCamera;

    [Header("Camera")]
    public LayerMask renderCullingMask;
    private Sprite endSprite;

    [Header("Visual")]
    public TextMeshProUGUI title;
    public Image backgroung;

    [Header("Buttons")]
    public Button menuBtn;
    public Button shareBtn;
    public Button nextLevelBtn;

    [Header("Stars")]
    public StarObject star1;
    public StarObject star2;
    public StarObject star3;
    [TextArea] public string finishLevelText;
    [TextArea] public string starJumpsTemplate;

    [Header("Other")]
    public TextMeshProUGUI jumpsText;
    public Image levelOverview;
    [TextArea] public string jumpsTemplate;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
        mainCamera = Camera.main;
    }

    void Start()
    {
        LevelManager.Instance.OnStarsChanged.AddListener(UpdateStars);
        star1.SetStarText(finishLevelText);
        star2.SetStarText(ProcessTemplate(starJumpsTemplate, LevelManager.Instance.data.secondStarMaxJumps));
        star3.SetStarText(ProcessTemplate(starJumpsTemplate, LevelManager.Instance.data.thirdStarMaxJumps));

        UpdateStars(LevelManager.Instance.currentStars);

        player.OnJump.AddListener(UpdateJumps);
        UpdateJumps(player.planetJumpsCounter);

        menuBtn.onClick.AddListener(LevelManager.Instance.GoToMenu);
        //shareBtn.onClick.AddListener();
        if (LevelManager.Instance.HasNextLevel()) nextLevelBtn.onClick.AddListener(LevelManager.Instance.GoToNextLevel);
        else nextLevelBtn.gameObject.SetActive(false);
    }

    string ProcessTemplate<T>(string template, T value)
    {
        int numStart = template.IndexOf('{');
        int numEnd = template.IndexOf('}');
        return template[..numStart] + value.ToString() + template.Substring(numEnd + 1, template.Length - numEnd - 1);
    }

    void UpdateJumps(uint jumps)
    {
        jumpsText.text = ProcessTemplate(jumpsTemplate, jumps);
    }

    void UpdateStars(int stars)
    {
        star3.SetStarActive(stars == 3);
        star2.SetStarActive(stars >= 2);
        star1.SetStarActive(stars >= 1);
    }

    public void Open()
    {
        levelOverview.sprite = CreateSprite();
    }

    Sprite CreateSprite()
    {
        RectTransform canvasRect = GetComponent<RectTransform>();
        RectTransform photoImageRect = levelOverview.GetComponent<RectTransform>();
        float imageAspect = photoImageRect.rect.width / photoImageRect.rect.height;
        int height = (int)canvasRect.rect.height;
        int width = (int)(height * imageAspect);

        RenderTexture renderTexture = new(width, height, 0, RenderTextureFormat.ARGB32);
        Texture2D texture = new(width, height, TextureFormat.RGBA32, false);

        RenderTexture lastRenderTex = mainCamera.targetTexture;
        mainCamera.targetTexture = renderTexture;
        LayerMask lastCullingMask = mainCamera.cullingMask;
        mainCamera.cullingMask = renderCullingMask;
        float lastAspect = mainCamera.aspect;
        mainCamera.aspect = imageAspect;
        mainCamera.Render();

        mainCamera.targetTexture = lastRenderTex;
        mainCamera.cullingMask = lastCullingMask;
        mainCamera.aspect = lastAspect;

        lastRenderTex = RenderTexture.active;
        RenderTexture.active = renderTexture;

        texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = lastRenderTex;

        return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), 0.5f * Vector2.one);
    }
}
