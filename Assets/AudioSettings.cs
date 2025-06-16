using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSettings : MonoBehaviour
{
    [SerializeField] private string settingName;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.mute = !SettingsManager.Instance.GetBool(settingName);
        SettingsManager.Instance.OnSettingsChanged.AddListener(AudioSettingChanged);
    }

    private void AudioSettingChanged(SettingsValueType type, string name)
    {
        if (name != settingName || type != SettingsValueType.BOOL) return;

        audioSource.mute = !SettingsManager.Instance.GetBool(settingName);
    }
}
