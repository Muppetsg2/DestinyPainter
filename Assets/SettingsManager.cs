using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private static SettingsManager instance;
    public static SettingsManager Instance => instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one SettingsManager found in scene");
            DestroyImmediate(this);
            return;
        }

        instance = this;
    }
}
