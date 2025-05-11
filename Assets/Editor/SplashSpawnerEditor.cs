#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplashSpawner))]
public class SplashSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SplashSpawner spawner = (SplashSpawner)target;
        if (GUILayout.Button("Spawn Splashes"))
        {
            spawner.SpawnSplashes();
        }
        if (GUILayout.Button("Delete Splashes"))
        {
            spawner.DeleteSplashes();
        }
    }
}
#endif