using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlanetRotation;

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

    private void Start()
    {
        pauseBtn.onClick.AddListener(LevelManager.Instance.PauseLevel);
        player.OnJump.AddListener(UpdateJumps);
        player.OnRotationChanged.AddListener(UpdateRotation);
        LevelManager.Instance.OnStarsChanged.AddListener(UpdateStars);

        UpdateRotation(RotationMode.None, player.rotationMode);
        UpdateJumps(player.planetJumpsCounter);
        UpdateStars(LevelManager.Instance.currentStars);
    }

    private void UpdateJumps(uint jumps)
    {
        jumpsText.text = jumps.ToString();
    }

    private void UpdateStars(int stars)
    {
        star3Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, stars == 3);
        star3Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, stars == 3);

        star2Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, stars >= 2);
        star2Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, stars >= 2);

        star1Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, stars >= 1);
        star1Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, stars >= 1);
    }

    private void UpdateRotation(RotationMode previous, RotationMode actual)
    {
        switch (actual)
        {
            case RotationMode.None:
            {
                rightBtn.interactable = true;
                leftBtn.interactable = true;
                break;
            }
            case RotationMode.CounterClockwise:
            {
                rightBtn.interactable = true;
                leftBtn.interactable = false;
                break;
            }
            case RotationMode.Clockwise:
            {
                rightBtn.interactable = false;
                leftBtn.interactable = true;
                break;
            }
        }
    }
}
