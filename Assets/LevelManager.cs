using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public LevelData data;
    public int currentStars;
    
    public string menuName;

    private void Update()
    {
        
    }

    public void FinishLevel()
    {
        data.SaveLevelStars(currentStars);
        SceneManager.LoadScene(menuName);
    }
}
