using UnityEngine;

public class UnlockAllCheat : MonoBehaviour
{
    public LevelsCollectionData levels;

    public void UnlockAll()
    {
        foreach (var level in levels)
        {
            if (level.GetLevelFinished()) continue;
            level.SaveLevelStars(0);
        }
    }
}
