using UnityEngine;
using TMPro;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MuralImageGeneratorSettings", menuName = "Scriptable Objects/MuralImageGeneratorSettings")]
public class MuralImageGeneratorSettings : ScriptableObject
{
    // Text
    [Header("Text")]
    [TextArea] public string text;
    public TMP_FontAsset font;
    public int textSize;
    public Color textColor;

    // Background
    [Header("Background")]
    [Range(0f, 370f)] public float backgroundRadius;
    public Color backgroundColor;
    [Range(0f, 100f)] public float backgroundBorder;

    // Splash
    [Header("Splash")]
    public int numberOfSplashes;
    public GameObject splashPrefab;
    [Range(0f, 1f)] public float splashAlpha;
    [Range(0f, 1f)] public float splashEdgeSharpness;
    public float splashScale;
    [Range(0f, 1f)] public float splashFillScale;

    // Random Scalers
    [Header("Random Scalers")]
    [Range(-1f, 1f)] public float circleAngleRandomScaler;
    [Range(-1f, 1f)] public float circleDistanceRandomScaler;

    // Splash Colors
    [Header("Splash Colors")]
    public List<Color> colorList;
}