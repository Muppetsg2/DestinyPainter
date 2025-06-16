using UnityEngine;
using UnityEngine.Events;

public enum SettingsValueType
{
    BOOL = 0,
    INT = 1,
    FLOAT = 2,
    STRING = 3
}

public class SettingsManager : MonoBehaviour
{
    private static SettingsManager instance;
    public static SettingsManager Instance => instance;

    public UnityEvent<SettingsValueType, string> OnSettingsChanged;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one SettingsManager found in scene");
            DestroyImmediate(this);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // BOOL
    public void SetBool(string name, bool value)
    {
        PlayerPrefs.SetInt(name, value ? 1 : 0);
        OnSettingsChanged?.Invoke(SettingsValueType.BOOL, name);
    }

    public bool GetBool(string name, bool defaultValue)
    {
        return PlayerPrefs.GetInt(name, defaultValue ? 1 : 0) == 1;
    }

    public bool GetBool(string name)
    {
        return GetBool(name, false);
    }

    // INT
    public void SetInt(string name, int value)
    {
        PlayerPrefs.SetInt(name, value);
        OnSettingsChanged?.Invoke(SettingsValueType.INT, name);
    }

    public int GetInt(string name, int defaultValue)
    {
        return PlayerPrefs.GetInt(name, defaultValue);
    }

    public int GetInt(string name)
    {
        return GetInt(name, 0);
    }

    // FLOAT
    public void SetFloat(string name, float value)
    {
        PlayerPrefs.SetFloat(name, value);
        OnSettingsChanged?.Invoke(SettingsValueType.FLOAT, name);
    }

    public float GetFloat(string name, float defaultValue)
    {
        return PlayerPrefs.GetFloat(name, defaultValue);
    }

    public float GetFloat(string name)
    {
        return GetFloat(name, 0f);
    }

    // STRING
    public void SetString(string name, string value)
    {
        PlayerPrefs.SetString(name, value);
        OnSettingsChanged?.Invoke(SettingsValueType.STRING, name);
    }

    public string GetString(string name, string defaultValue)
    {
        return PlayerPrefs.GetString(name, defaultValue);
    }

    public string GetString(string name)
    {
        return GetString(name, "");
    }

    // RESET SETTING
    public void ResetSetting(string name)
    {
        PlayerPrefs.DeleteKey(name);
    }
}
