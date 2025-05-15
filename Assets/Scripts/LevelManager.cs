using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public PlayerController player;
    public LevelData data;
    public int currentStars;
    
    public string menuName;

    private void Start()
    {
        if (instance != null) Destroy(this);
        instance = this;
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

    public void GotoMenu()
    {
        SceneManager.LoadScene(menuName);
    }
}
