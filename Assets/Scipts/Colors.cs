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

    public static Dictionary<ColorType, float3> colorsDict = new Dictionary<ColorType, float3>()
    {
        {ColorType.Red, new float3(1,0.09f,0) },
        {ColorType.Violet, new float3(0.62f,0,1) }
    };
}
