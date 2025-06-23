using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "LevelsCollectionData", menuName = "Game/Create Levels Collection Data")]
public class LevelsCollectionData : ScriptableObject, IEnumerable<LevelData>
{
    [Header("Collection")]
    public List<LevelData> levels;

    public IEnumerator<LevelData> GetEnumerator()
    {
        return levels.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
