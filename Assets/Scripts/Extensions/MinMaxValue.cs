using System;
using UnityEngine;

[Serializable]
public struct MinMaxValue<T>
{
    [SerializeField] public T Min;
    [SerializeField] public T Max;
}
