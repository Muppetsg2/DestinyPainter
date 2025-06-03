using DG.Tweening;
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
    public Transform buttons;
    public GameObject photo;
    public GridLayoutGroup starsLayout;

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

    [Header("Show Animation")]
    public float showItemsFromBottomTime;
    public AnimationCurve showItemsFromBottomCurve;
    public float showFromPlayerViewTime;
    public AnimationCurve showFromPlayerViewCurve;
    public float showFromPlayerViewScale;
    public float showFromPlayerViewStartYOffset;
    public float showFromPlayerViewStarsDelay;
    public float photoImageColorTransitionTime;

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
        float bottomY = -GetComponent<RectTransform>().rect.height;

        Vector3 lastPos = title.transform.localPosition;
        float titleLocalY = lastPos.y;
        lastPos.y = bottomY;
        title.transform.localPosition = lastPos;

        lastPos = buttons.transform.localPosition;
        float buttonsLocalY = lastPos.y;
        lastPos.y = bottomY;
        buttons.transform.localPosition = lastPos;

        menuBtn.enabled = false;
        shareBtn.enabled = false;
        nextLevelBtn.enabled = false;

        endSprite = CreateSprite();
        levelOverview.sprite = endSprite;
        levelOverview.color = Color.black;

        Vector3 scale = Vector3.one * showFromPlayerViewScale;

        photo.transform.localScale = scale;
        lastPos = photo.transform.localPosition;
        float photoLocalY = lastPos.y;
        lastPos.y += showFromPlayerViewStartYOffset;
        photo.transform.localPosition = lastPos;
        photo.SetActive(false);

        starsLayout.enabled = false;

        star1.transform.localScale = scale;
        star1.SetActiveTextHolder(false);
        lastPos = star1.transform.localPosition;
        float star1LocalY = lastPos.y;
        lastPos.y += showFromPlayerViewStartYOffset;
        star1.transform.localPosition = lastPos;
        star1.gameObject.SetActive(false);

        star2.transform.localScale = scale;
        star2.SetActiveTextHolder(false);
        lastPos = star2.transform.localPosition;
        float star2LocalY = lastPos.y;
        lastPos.y += showFromPlayerViewStartYOffset;
        star2.transform.localPosition = lastPos;
        star2.gameObject.SetActive(false);

        star3.transform.localScale = scale;
        star3.SetActiveTextHolder(false);
        lastPos = star3.transform.localPosition;
        float star3LocalY = lastPos.y;
        lastPos.y += showFromPlayerViewStartYOffset;
        star3.transform.localPosition = lastPos;
        star3.gameObject.SetActive(false);

        jumpsText.transform.localScale = scale;
        lastPos = jumpsText.transform.localPosition;
        float jumpsTextLocalY = lastPos.y;
        lastPos.y += showFromPlayerViewStartYOffset;
        jumpsText.transform.localPosition = lastPos;
        jumpsText.gameObject.SetActive(false);

        title.transform.DOLocalMoveY(titleLocalY, showItemsFromBottomTime).SetEase(showItemsFromBottomCurve).OnComplete(() =>
        {
            photo.SetActive(true);
            photo.transform.DOLocalMoveY(photoLocalY, showFromPlayerViewTime).SetEase(showFromPlayerViewCurve);
            photo.transform.DOScale(1f, showFromPlayerViewTime).SetEase(showFromPlayerViewCurve).OnComplete(() =>
            {
                levelOverview.DOColor(Color.white, photoImageColorTransitionTime);

                star1.gameObject.SetActive(true);
                star1.transform.DOLocalMoveY(star1LocalY, showFromPlayerViewTime).SetEase(showFromPlayerViewCurve);
                star1.transform.DOScale(1f, showFromPlayerViewTime).SetEase(showFromPlayerViewCurve).OnComplete(() =>
                {
                    star1.SetActiveTextHolder(true);
                });

                star2.transform.DOLocalMoveY(star1LocalY, showFromPlayerViewTime).SetEase(showFromPlayerViewCurve).SetDelay(showFromPlayerViewStarsDelay)
                .OnStart(() => star2.gameObject.SetActive(true));
                star2.transform.DOScale(1f, showFromPlayerViewTime).SetEase(showFromPlayerViewCurve).SetDelay(showFromPlayerViewStarsDelay).OnComplete(() =>
                {
                    star2.SetActiveTextHolder(true);
                });

                star3.transform.DOLocalMoveY(star1LocalY, showFromPlayerViewTime).SetEase(showFromPlayerViewCurve).SetDelay(2 * showFromPlayerViewStarsDelay)
                .OnStart(() => star3.gameObject.SetActive(true));
                star3.transform.DOScale(1f, showFromPlayerViewTime).SetEase(showFromPlayerViewCurve).SetDelay(2 * showFromPlayerViewStarsDelay).OnComplete(() =>
                {
                    star3.SetActiveTextHolder(true);

                    starsLayout.enabled = true;

                    jumpsText.gameObject.SetActive(true);

                    jumpsText.transform.DOLocalMoveY(jumpsTextLocalY, showFromPlayerViewTime).SetEase(showFromPlayerViewCurve);
                    jumpsText.transform.DOScale(1f, showFromPlayerViewTime).SetEase(showFromPlayerViewCurve).OnComplete(() =>
                    {
                        buttons.transform.DOLocalMoveY(buttonsLocalY, showItemsFromBottomTime).SetEase(showItemsFromBottomCurve).OnComplete(() =>
                        {
                            menuBtn.enabled = true;
                            shareBtn.enabled = true;
                            nextLevelBtn.enabled = true;
                        });
                    });
                });
            });
        });
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

        float lastAspect = mainCamera.aspect;
        mainCamera.aspect = imageAspect;

        RenderTexture lastRenderTex = mainCamera.targetTexture;
        mainCamera.targetTexture = renderTexture;
        LayerMask lastCullingMask = mainCamera.cullingMask;
        mainCamera.cullingMask = renderCullingMask;

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
