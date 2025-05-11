using System.Collections.Generic;
using UnityEngine;


public class ColorsManager : MonoBehaviour
{

    private static ColorsManager instance;

    public static ColorsManager Instance {  get { return instance; } }

    public enum ColorType
    {
        None,
        Red,
        Violet,
        Blue
    }

    [System.Serializable]
    public struct ColorStruct
    {
        public ColorType colorName;
        public Color color;
        public Color secondaryColor;
    }

    public ColorStruct color1;
    public ColorStruct color2;
    public ColorStruct color3;

    public Material material1;
    public Material material2;
    public Material material3;

    public Dictionary<ColorType, Color> colorsDict = new Dictionary<ColorType, Color>();

    public Dictionary<ColorType, Color> secondaryColorsDict = new Dictionary<ColorType, Color>();

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one ColorsManager in the Scene");
            DestroyImmediate(this);
        }

        instance = this;
        InitializeColors();
    }

    private void InitializeColors()
    {
        colorsDict.Add(color1.colorName, color1.color);
        colorsDict.Add(color2.colorName, color2.color);
        colorsDict.Add(color3.colorName, color3.color);

        secondaryColorsDict.Add(color1.colorName, color1.secondaryColor);
        secondaryColorsDict.Add(color2.colorName, color2.secondaryColor);
        secondaryColorsDict.Add(color3.colorName, color3.secondaryColor);

        material1.SetColor("_Center", color1.secondaryColor);
        material1.SetColor("_Outer", color1.color);

        material2.SetColor("_Center", color2.secondaryColor);
        material2.SetColor("_Outer", color2.color);

        material3.SetColor("_Center", color3.secondaryColor);
        material3.SetColor("_Outer", color3.color);
    }

    public Color GetPrimaryColor(ColorType type)
    {
        return colorsDict[type];
    }

    public Color GetSeconderyColor(ColorType type)
    {
        return secondaryColorsDict[type];
    }
}
