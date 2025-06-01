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

    private Sequence sq = null;

    void Awake()
    {
        Setup();
    }

#if UNITY_EDITOR
    public void Setup()
#else
    private void Setup()
#endif
    {
        float minLength = lineRenderer.GetPosition(0).y;
        float maxLength = lineRenderer.GetPosition(lineRenderer.positionCount - 1).y;
        float maxmindiv = 1f / (maxLength - minLength);
        for (int i = 0; i < lineRenderer.positionCount; ++i)
        {
            positionPercent.Add((lineRenderer.GetPosition(i).y - minLength) * maxmindiv);
        }

        //lineRenderer.widthMultiplier = 0f;
        SetLineLength(0f);

        //PlayAnim();
    }

    public void SetColor(Color col)
    {
        GradientColorKey[] colorKeys = lineRenderer.colorGradient.colorKeys;
        for (int i = 0; i < lineRenderer.colorGradient.colorKeys.Length; ++i)
        {
            GradientColorKey key = colorKeys[i];
            key.color.r = col.r;
            key.color.g = col.g;
            key.color.b = col.b;
            colorKeys[i] = key;
        }
        GradientAlphaKey[] alphaKeys = lineRenderer.colorGradient.alphaKeys;
        for (int i = 0; i < lineRenderer.colorGradient.colorKeys.Length; ++i)
        {
            GradientAlphaKey key = alphaKeys[i];
            key.alpha = col.a;
            alphaKeys[i] = key;
        }
        Gradient gradient = new Gradient();
        gradient.SetKeys(colorKeys, alphaKeys);
        lineRenderer.colorGradient = gradient;
    }

    public void PlayAnim()
    {
        lineRenderer.widthMultiplier = Random.Range(lineWidth.Min, lineWidth.Max);

        sq = DOTween.Sequence().Pause();
        sq.Append(DOTween.To(
            () => currentLineLength,
            SetLineLength,
            Random.Range(lineLength.Min, lineLength.Max),
            Random.Range(animationTime.Min, animationTime.Max)
        ));

        sq.Play();
    }

    public void Complete()
    {
        if (sq != null)
        {
            sq.Kill(true);
            return;
        }

        lineRenderer.widthMultiplier = Random.Range(lineWidth.Min, lineWidth.Max);
        SetLineLength(Random.Range(lineLength.Min, lineLength.Max));
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
