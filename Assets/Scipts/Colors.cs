using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Colors
{
    public enum ColorType
    {
        None,
        Red,
        Violet
    }

    public static Dictionary<ColorType, Vector4> colorsDict = new Dictionary<ColorType, Vector4>()
    {
        {ColorType.Red, new Vector4(1,0.09f,0, 1) },
        {ColorType.Violet, new Vector4(0.62f,0,1, 1) }
    };
}
