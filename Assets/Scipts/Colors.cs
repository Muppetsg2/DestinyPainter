using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Colors
{
    public enum ColorType
    {
        None,
        Red,
        Blue
    }

    public static Dictionary<ColorType, float3> colorsDict = new Dictionary<ColorType, float3>()
    {
        {ColorType.Red, new float3(1,0,0) },
        {ColorType.Blue, new float3(0,0,1) }
    };
}
