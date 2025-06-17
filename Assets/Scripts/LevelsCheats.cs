using UnityEngine;

public class LevelsCheats : MonoBehaviour
{
    public LevelsCollectionData levels;

    public void UnlockAll()
    {
        foreach (LevelData level in levels)
        {
            if (level.GetLevelFinished()) continue;
            level.SaveLevelStars(0);
        }
    }

    public void LockAll()
    {
        foreach (LevelData level in levels)
        {
            if (!level.GetLevelFinished()) continue;
            level.ResetLevel();
        }
    }
}
