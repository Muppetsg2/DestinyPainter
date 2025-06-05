using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ScaleAnimator : MonoBehaviour
{
    [SerializeField] private Transform effectTransform;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private MinMaxValue<float> scaleFactor = new MinMaxValue<float>
    {
        Min = 1f,
        Max = 2f
    };
    [SerializeField] private float time = 1f;

    public UnityEvent OnAnimationStart;
    public UnityEvent OnAnimationComplete;

    public void PlayAnimation()
    {
        if (effectTransform == null || curve == null)
        {
            Debug.LogWarning("Curve or Transform is null.");
            return;
        }

        Vector3 originalScale = effectTransform.localScale;
        float t = 0f;

        OnAnimationStart?.Invoke();

        DOTween.To(() => t, x =>
        {
            t = x;
            float curveValue = curve.Evaluate(t / time);
            float remappedScaleFactor = Mathf.Lerp(scaleFactor.Min, scaleFactor.Max, curveValue.Remap(-1f, 1f, 0f, 1f));
            effectTransform.localScale = originalScale * remappedScaleFactor;

        }, time, time)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            effectTransform.localScale = originalScale;
            OnAnimationComplete?.Invoke();
        });
    }
}
