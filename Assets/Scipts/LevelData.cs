using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Create Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Menu:")]
    public string sceneName;

    [Header("Game:")]
    public int secondStarMaxJumps = 1;
    public int thirdStarMaxJumps = 0;

    public void LoadLevel()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SaveLevelStars(int stars)
    {
        if (stars < 0) stars = 0;
        if (stars > 3) stars = 3;

        PlayerPrefs.SetInt(sceneName, stars);
    }

    public int GetLevelStars()
    {
        return PlayerPrefs.GetInt(sceneName, 0);
    }
}
