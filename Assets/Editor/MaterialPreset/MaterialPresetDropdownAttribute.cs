using UnityEngine;

public class MaterialPresetDropdownAttribute : PropertyAttribute
{
    public string FontAssetFieldName { get; }

    public MaterialPresetDropdownAttribute(string fontAssetFieldName)
    {
        FontAssetFieldName = fontAssetFieldName;
    }
}