using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvasScript : MonoBehaviour
{
    public PlayerController player;

    [Header("Buttons")]
    public Button jumpBtn;
    public Button rightBtn;
    public Button leftBtn;
    public Button pauseBtn;

    [Header("Jumps Counter")]
    public TextMeshProUGUI jumpsText;

    [Header("Stars")]
    public Image star1Image;
    public Image star2Image;
    public Image star3Image;

    public Shadow star1Shadow;
    public Shadow star2Shadow;
    public Shadow star3Shadow;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();

        jumpBtn.onClick.AddListener(player.Launch);
        rightBtn.onClick.AddListener(player.RotateClockwise);
        leftBtn.onClick.AddListener(player.RotateCounterClockwise);
    }

    void Start()
    {
        pauseBtn.onClick.AddListener(LevelManager.Instance.PauseLevel);
    }

    void Update()
    {
        UpdateJumps();
        UpdateStars();
    }

    void UpdateJumps()
    {
        jumpsText.text = player.planetJumpsCounter.ToString();
    }

    void UpdateStars()
    {
        if (LevelManager.Instance.currentStars == 3)
        {
            star3Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, true);
            star2Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, true);
            star1Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, true);

            star3Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, true);
            star2Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, true);
            star1Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, true);
        }
        else if (LevelManager.Instance.currentStars == 2)
        {
            star3Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, false);
            star2Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, true);
            star1Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, true);

            star3Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, false);
            star2Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, true);
            star1Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, true);
        }
        else if (LevelManager.Instance.currentStars == 1)
        {
            star3Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, false);
            star2Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, false);
            star1Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, true);

            star3Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, false);
            star2Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, false);
            star1Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, true);
        }
        else if (LevelManager.Instance.currentStars == 0)
        {
            star3Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, false);
            star2Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, false);
            star1Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, false);

            star3Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, false);
            star2Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, false);
            star1Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, false);
        }
    }
}
