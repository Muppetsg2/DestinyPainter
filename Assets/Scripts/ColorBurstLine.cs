using DG.Tweening;
using System.Net.Sockets;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ColorBurstLine : MonoBehaviour
{
    public AnimationCurve animationCurve;
    public MinMaxValue<float> width = new()
    {
        Min = 0.044f,
        Max = 0.205f
    };
    public float timeAlive = 0.25f;
    public float fadeAnimationDuration = 0.5f;

    public void PlayAnimation(Vector3 startPos, Vector3 endPos, Color start, Color end, float duration, float widthMultiplier = 1.0f)
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, startPos);

        lr.startColor = start;
        lr.endColor = end;

        float mw = width.Max - (width.Max - width.Min) * (1.0f - widthMultiplier);

        AnimationCurve scaledCurve = new AnimationCurve();
        foreach (Keyframe key in animationCurve.keys)
        {
            float scaledValue = Mathf.Lerp(width.Min, mw, key.value);
            Keyframe newKey = new Keyframe(key.time, scaledValue, key.inTangent, key.outTangent);
            scaledCurve.AddKey(newKey);
        }
        lr.widthCurve = scaledCurve;

        Sequence seq = DOTween.Sequence().Pause();

        seq.Append(DOTween.To(() => 0f, t =>
        {
            float curveValue = animationCurve.Evaluate(t);
            Vector3 newPos = Vector3.Lerp(startPos, endPos, curveValue);
            lr.SetPosition(1, newPos);
        }, 1f, duration));

        seq.AppendInterval(timeAlive);

        seq.Append(DOTween.To(() => 1f, t =>
        {
            Vector3 newPos = Vector3.Lerp(startPos, endPos, t);
            lr.SetPosition(1, newPos);
            lr.widthMultiplier = t;
        }, 0f, fadeAnimationDuration));

        seq.AppendInterval(0.01f);

        seq.OnComplete(() =>
        {
            DestroyImmediate(gameObject);
        });

        seq.Play();
    }
}