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
    private struct ColorStruct
    {
        public ColorType colorName;
        public Color color;
        public Color secondaryColor;
    }

    [Header("Colors")]
    [SerializeField] private ColorStruct color1;
    [SerializeField] private ColorStruct color2;
    [SerializeField] private ColorStruct color3;

    [Header("Material")]
    [SerializeField] private Material baseMaterial;

    private Dictionary<ColorType, Color> colorsDict = new Dictionary<ColorType, Color>();

    private Dictionary<ColorType, Color> secondaryColorsDict = new Dictionary<ColorType, Color>();

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

    private void OnValidate()
    {
        InitializeColors();
    }

    private void InitializeColors()
    {
        colorsDict.Clear();
        colorsDict.Add(ColorType.None, Color.black);
        colorsDict.Add(color1.colorName, color1.color);
        colorsDict.Add(color2.colorName, color2.color);
        colorsDict.Add(color3.colorName, color3.color);

        secondaryColorsDict.Clear();
        secondaryColorsDict.Add(ColorType.None, Color.black);
        secondaryColorsDict.Add(color1.colorName, color1.secondaryColor);
        secondaryColorsDict.Add(color2.colorName, color2.secondaryColor);
        secondaryColorsDict.Add(color3.colorName, color3.secondaryColor);
    }

    public Color GetPrimaryColor(ColorType type)
    {
        return colorsDict[type];
    }

    public Color GetSecondaryColor(ColorType type)
    {
        return secondaryColorsDict[type];
    }

    public Material GetMaterial(ColorType type)
    {
        Material mat = new Material(baseMaterial);
        mat.SetColor("_Outer", GetPrimaryColor(type));
        mat.SetColor("_Center", GetSecondaryColor(type));

        return mat;
    }
}
