using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public PlayerController player;
    public LevelData data;
    public int currentStars;

    public string menuName;

    public CameraPlanetSnap cameraSnap;

    [Header("Canvases")]
    public GameCanvasScript gameCanvas;
    public PauseCanvasScript pauseCanvas;
    public EndCanvasScript endCanvas;

    public UnityEvent<int> OnStarsChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        player.OnJump.AddListener(UpdateStars);
        UpdateStars(player.planetJumpsCounter);
    }

    private void UpdateStars(uint playerJumps)
    {
        int oldStars = currentStars;
        if (playerJumps <= data.thirdStarMaxJumps)
        {
            currentStars = 3;
        }
        else if (playerJumps <= data.secondStarMaxJumps)
        {
            currentStars = 2;
        }
        else
        {
            currentStars = 1;
        }

        if (currentStars != oldStars)
        {
            OnStarsChanged.Invoke(currentStars);
        }
    }

    public void RestartLevel()
    {
        data.LoadLevel();
    }

    public void UnpauseLevel()
    {
        pauseCanvas.Close(() =>
        {
            pauseCanvas.gameObject.SetActive(false);
            gameCanvas.gameObject.SetActive(true);
        });
    }

    public void PauseLevel()
    {
        gameCanvas.gameObject.SetActive(false);
        pauseCanvas.gameObject.SetActive(true);
        pauseCanvas.Open();
    }

    public void FinishLevel()
    {
        data.SaveLevelStars(currentStars);
        gameCanvas.gameObject.SetActive(false);
        cameraSnap.PlayEndAnim(() =>
        {
            endCanvas.gameObject.SetActive(true);
            endCanvas.Open();
        });
    }

    public bool HasNextLevel()
    {
        return data.nextLevel != null;
    }

    public void GoToNextLevel()
    {
        data.nextLevel.LoadLevel();
    }

    public void GoToMenu()
    {
        LevelLoader.Instance.LoadLevel(menuName);
    }
}
