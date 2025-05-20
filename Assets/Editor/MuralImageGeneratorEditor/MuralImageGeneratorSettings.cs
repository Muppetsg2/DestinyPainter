using UnityEngine;
using TMPro;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MuralImageGeneratorSettings", menuName = "Scriptable Objects/MuralImageGeneratorSettings")]
public class MuralImageGeneratorSettings : ScriptableObject
{
    // Text
    [Header("Text")]
    public string text;
    public TMP_FontAsset font;
    public int textSize;
    public Color textColor;

    // Background
    [Header("Background")]
    public Sprite backgroundSprite;
    public Color backgroundColor;
    [Range(0f, 100f)] public float backgroundBorder;

    // Splash
    [Header("Splash")]
    public int numberOfSplashes;
    public Sprite splashSprite;
    public Material splashMaterial;
    [Range(0f, 1f)] public float splashAlpha;
    [Range(0f, 1f)] public float splashEdgeSharpness;
    [Range(0f, 1f)] public float splashScale;
    [Range(0f, 1f)] public float splashFillScale;

    // Splash Colors
    [Header("Splash Colors")]
    public List<Color> colorList;
}