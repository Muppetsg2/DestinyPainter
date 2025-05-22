using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SplashLineAnimation : MonoBehaviour
{
    public MinMaxValue<float> lineLength;
    public MinMaxValue<float> lineWidth;

    public MinMaxValue<float> animationTime;

    public LineRenderer lineRenderer;

    private float currentLineLength;
    private readonly List<float> positionPercent = new();

    void Start()
    {
        float minLength = lineRenderer.GetPosition(0).y;
        float maxLength = lineRenderer.GetPosition(lineRenderer.positionCount - 1).y;
        float maxmindiv = 1f / (maxLength - minLength);
        for (int i = 0; i < lineRenderer.positionCount; ++i)
        {
            positionPercent.Add((lineRenderer.GetPosition(i).y - minLength) * maxmindiv);
        }

        lineRenderer.widthMultiplier = 0f;
        SetLineLength(0f);

        PlayAnim();
    }

    public void SetColor(ColorType color)
    {
        Color primary = ColorsManager.Instance.GetSecondaryColor(color);

        GradientColorKey[] keys = lineRenderer.colorGradient.colorKeys;
        for (int i = 0; i < lineRenderer.colorGradient.colorKeys.Length; ++i)
        {
            GradientColorKey key = keys[i];
            key.color.r = primary.r;
            key.color.g = primary.g;
            key.color.b = primary.b;
            keys[i] = key;
        }
        Gradient gradient = new Gradient();
        gradient.SetKeys(keys, lineRenderer.colorGradient.alphaKeys);
        lineRenderer.colorGradient = gradient;
    }

    public void PlayAnim()
    {
        lineRenderer.widthMultiplier = Random.Range(lineWidth.Min, lineWidth.Max);

        DOTween.To(
            () => currentLineLength,
            SetLineLength,
            Random.Range(lineLength.Min, lineLength.Max),
            Random.Range(animationTime.Min, animationTime.Max)
        );
    }

    void SetLineLength(float length)
    {
        currentLineLength = length;
        for (int i = 0; i < lineRenderer.positionCount; ++i)
        {
            lineRenderer.SetPosition(i, positionPercent[i] * length * Vector3.down);
        }
    }
}
