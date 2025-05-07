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
        Violet
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

    public Material material1;
    public Material material2;

    public static Dictionary<ColorType, Vector4> colorsDict = new Dictionary<ColorType, Vector4>();

    public static Dictionary<ColorType, Vector4> secondaryColorsDict = new Dictionary<ColorType, Vector4>();

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
        }

        instance = this;
        InitializeColors();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitializeColors()
    {
        colorsDict.Add(color1.colorName, color1.color);
        colorsDict.Add(color2.colorName, color2.color);

        secondaryColorsDict.Add(color1.colorName, color1.secondaryColor);
        secondaryColorsDict.Add(color2.colorName, color2.secondaryColor);

        material1.SetColor("_Center", color1.secondaryColor);
        material1.SetColor("_Outer", color1.color);

        material2.SetColor("_Center", color2.secondaryColor);
        material2.SetColor("_Outer", color2.color);
    }
}
