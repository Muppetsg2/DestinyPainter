using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ColorBurstLine : MonoBehaviour
{
    public AnimationCurve animationCurve;
    public float minWidth = 0.044f;
    public float maxWidth = 0.205f;

    public void PlayAnimation(Vector3 startPos, Vector3 endPos, Color start, Color end, float duration, float widthMultiplier = 1.0f)
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, startPos);

        lr.startColor = start;
        lr.endColor = end;

        float mw = maxWidth - (maxWidth - minWidth) * (1.0f - widthMultiplier);

        AnimationCurve scaledCurve = new AnimationCurve();
        foreach (Keyframe key in animationCurve.keys)
        {
            float scaledValue = Mathf.Lerp(minWidth, mw, key.value);
            Keyframe newKey = new Keyframe(key.time, scaledValue, key.inTangent, key.outTangent);
            scaledCurve.AddKey(newKey);
        }
        lr.widthCurve = scaledCurve;

        DOTween.To(() => 0f, t =>
        {
            float curveValue = animationCurve.Evaluate(t);

            Vector3 newPos = Vector3.Lerp(startPos, endPos, curveValue);
            lr.SetPosition(1, newPos);
        }, 1f, duration);
    }
}