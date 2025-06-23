using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SettingsButton : MonoBehaviour
{
    [System.Serializable]
    public enum SettingsBtnType
    {
        Audio = 0
    }

    [SerializeField] private SettingsBtnType type;
    [SerializeField] private string settingName;

    [LayoutShowIf(nameof(type), SettingsBtnType.Audio)]
    [LayoutStart("Audio Button Settings", ELayout.FoldoutBox)]
    [SerializeField, ShowIf(nameof(type), SettingsBtnType.Audio)] private Image image;
    [SerializeField, ShowIf(nameof(type), SettingsBtnType.Audio)] private Sprite on;
    [SerializeField, ShowIf(nameof(type), SettingsBtnType.Audio)] private Sprite off;
    [LayoutEnd(".")]

    private Button button;
    private bool bvalue = true;

    void Start()
    {
        button = GetComponent<Button>();

        switch (type)
        {
            case SettingsBtnType.Audio:
            {
                button.onClick.AddListener(() =>
                {
                    SettingsManager.Instance.SetBool(settingName, !bvalue);
                    AudioUpdateValue();
                });
                AudioUpdateValue();
                break;
            }
        }
    }

    private void AudioUpdateValue()
    {
        bvalue = SettingsManager.Instance.GetBool(settingName, true);
        image.sprite = bvalue ? on : off;
    }
}
