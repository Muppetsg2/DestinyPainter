using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public PlayerController player;
    public LevelData data;
    public int currentStars;

    public LevelData nextLevel;

    public string menuName;

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

    public void FinishLevel()
    {
        data.SaveLevelStars(currentStars);
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
