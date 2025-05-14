using UnityEngine;
using System.Collections.Generic;

public enum ColorType
{
    None,
    Red,
    Violet,
    Blue
}

[CreateAssetMenu(fileName = "ColorPalette", menuName = "Color System/Color Palette")]
public class ColorPalette : ScriptableObject
{
    [System.Serializable]
    public class ColorEntry
    {
        public ColorType colorType;
        public Color primaryColor;
        public Color secondaryColor;
    }

    [SerializeField]
    private List<ColorEntry> colors = new List<ColorEntry>();

    private Dictionary<ColorType, ColorEntry> colorDict = new Dictionary<ColorType, ColorEntry>();

    public void Initialize()
    {
        foreach (var entry in colors)
        {
            if (!colorDict.ContainsKey(entry.colorType))
                colorDict.Add(entry.colorType, entry);
            else
                colorDict[entry.colorType] = entry;
        }
    }

    public Color GetPrimaryColor(ColorType type)
    {
        if (colorDict == null) Initialize();
        return colorDict.TryGetValue(type, out var entry) ? entry.primaryColor : Color.black;
    }

    public Color GetSecondaryColor(ColorType type)
    {
        if (colorDict == null) Initialize();
        return colorDict.TryGetValue(type, out var entry) ? entry.secondaryColor : Color.black;
    }
}