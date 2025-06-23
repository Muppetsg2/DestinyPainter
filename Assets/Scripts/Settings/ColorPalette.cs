using UnityEngine;
using System.Collections.Generic;

public enum ColorType
{
    None,
    Red,
    Violet,
    Blue
}

public enum ColorCategory
{
    Primary,
    Secondary,
    PrimaryHDR,
    SecondaryHDR
}

[CreateAssetMenu(fileName = "ColorPalette", menuName = "Color System/Color Palette")]
public class ColorPalette : ScriptableObject
{
    [System.Serializable]
    public struct ColorEntry
    {
        public ColorType colorType;
        public Color primaryColor;
        public Color secondaryColor;
        [ColorUsage(true, true)]
        public Color primaryHDRColor;
        [ColorUsage(true, true)]
        public Color secondaryHDRColor;
    }

    [SerializeField]
    private List<ColorEntry> colors = new List<ColorEntry>();

    [Header("Star Active")]
    public Color primaryStarActiveColor = Color.yellow;
    public Color secondaryStarActiveColor = Color.yellow;
    [ColorUsage(true, true)]
    public Color primaryStarActiveHDRColor = Color.yellow;
    [ColorUsage(true, true)]
    public Color secondaryStarActiveHDRColor = Color.yellow;

    [Header("Star Inactive")]
    public Color primaryStarInactiveColor = Color.gray;
    public Color secondaryStarInactiveColor = Color.gray;
    [ColorUsage(true, true)]
    public Color primaryStarInactiveHDRColor = Color.gray;
    [ColorUsage(true, true)]
    public Color secondaryStarInactiveHDRColor = Color.gray;

    [Header("Background")]
    public Color backgroundColor = Color.black;

    [System.NonSerialized]
    private Dictionary<ColorType, ColorEntry> colorDict = null;

    [System.NonSerialized]
    private bool isInitialized = false;

    public void Initialize()
    {
        if (isInitialized) return;
        colorDict = new Dictionary<ColorType, ColorEntry>();
        foreach (var entry in colors)
        {
            if (!colorDict.ContainsKey(entry.colorType))
                colorDict.Add(entry.colorType, entry);
        }
        isInitialized = true;
    }

    public bool HasColorType(ColorType type)
    {
        if (colorDict == null) Initialize();
        return colorDict.ContainsKey(type);
    }

    public Color GetColor(ColorType type, ColorCategory category)
    {
        if (colorDict == null) Initialize();

        if (!colorDict.TryGetValue(type, out var entry)) return Color.black;

        return category switch
        {
            ColorCategory.Primary => entry.primaryColor,
            ColorCategory.Secondary => entry.secondaryColor,
            ColorCategory.PrimaryHDR => entry.primaryHDRColor,
            ColorCategory.SecondaryHDR => entry.secondaryHDRColor,
            _ => Color.black
        };
    }
}