using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    [System.Serializable]
    public enum SettingsBtnType
    {
        Audio = 0
    }

    [SerializeField] private SettingsBtnType type;
    [SerializeField] private string optionName;

    [LayoutShowIf(nameof(type), SettingsBtnType.Audio)]
    [LayoutStart("Audio Button Settings", ELayout.FoldoutBox)]
    [SerializeField, ShowIf(nameof(type), SettingsBtnType.Audio)] private Image image;
    [SerializeField, ShowIf(nameof(type), SettingsBtnType.Audio)] private Sprite on;
    [SerializeField, ShowIf(nameof(type), SettingsBtnType.Audio)] private Sprite off;
    [LayoutEnd(".")]

    void Start()
    {
        
    }
}
