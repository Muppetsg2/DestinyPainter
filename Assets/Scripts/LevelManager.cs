using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public PlayerController player;
    public LevelData data;
    public int currentStars;

    public LevelData nextLevel;

    public string menuName;

    public CameraPlanetSnap cameraSnap;

    [Header("Canvases")]
    public GameObject gameCanvas;
    public GameObject pauseCanvas;
    public GameObject endCanvas;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (player.planetJumpsCounter <= data.thirdStarMaxJumps)
        {
            currentStars = 3;
        }
        else if (player.planetJumpsCounter <= data.secondStarMaxJumps)
        {
            currentStars = 2;
        }
        else
        {
            currentStars = 1;
        }
    }

    public void RestartLevel()
    {
        data.LoadLevel();
    }

    public void UnpauseLevel()
    {
        pauseCanvas.SetActive(false);
        gameCanvas.SetActive(true);
    }

    public void PauseLevel()
    {
        gameCanvas.SetActive(false);
        pauseCanvas.SetActive(true);
    }

    public void FinishLevel()
    {
        data.SaveLevelStars(currentStars);
        gameCanvas.SetActive(false);
        cameraSnap.PlayEndAnim(() =>
        {
            endCanvas.SetActive(true);
        });
    }

    public bool HasNextLevel()
    {
        return nextLevel != null;
    }

    public void GoToNextLevel()
    {
        nextLevel.LoadLevel();
    }

    public void GoToMenu()
    {
        LevelLoader.Instance.LoadLevel(menuName);
    }
}
