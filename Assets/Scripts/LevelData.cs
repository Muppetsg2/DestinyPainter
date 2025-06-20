using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Create Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Hierarchy")]
    public LevelData previousLevel;
    public LevelData nextLevel;

    [Header("Menu:")]
    public string sceneName;

    [Header("Game:")]
    public int secondStarMaxJumps = 1;
    public int thirdStarMaxJumps = 0;

    [Header("ShareImage:")]
    public int levelNum;

    public UnityEvent OnLevelDataChanged;

    public void LoadLevel()
    {
        LevelLoader.Instance.LoadLevel(sceneName);
    }

    public void SaveLevelStars(int stars)
    {
        if (stars < 0) stars = 0;
        if (stars > 3) stars = 3;

        string key = sceneName + "_stars";
        if (PlayerPrefs.HasKey(key))
        {
            if (stars <= PlayerPrefs.GetInt(key)) return;
        }
        PlayerPrefs.SetInt(key, stars);

        OnLevelDataChanged?.Invoke();
    }

    public int GetLevelStars()
    {
        return PlayerPrefs.GetInt(sceneName + "_stars", 0);
    }

    public bool GetLevelFinished()
    {
        return PlayerPrefs.HasKey(sceneName + "_stars");
    }

    public void ResetLevel()
    {
        PlayerPrefs.DeleteKey(sceneName + "_stars");
        OnLevelDataChanged?.Invoke();
    }
}
