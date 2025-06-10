using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Animation Settings")]
    [SerializeField] private Transform effectTransform;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float pressedScaleFactor = 0.5f;
    [SerializeField] private float time = 0.2f;

    private Button button;
    private Vector3 originalScale;
    private Sequence sq = null;
    private bool up = false;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        if (effectTransform != null) originalScale = effectTransform.localScale;
    }

    public void PlayPressDownAnimation()
    {
        PlayScaleAnimation(originalScale.x, pressedScaleFactor);
    }

    public void PlayPressExitAnimation()
    {
        PlayScaleAnimation(effectTransform.localScale.x, originalScale.x);
    }

    private void PlayScaleAnimation(float fromScale, float toScale)
    {
        if (effectTransform == null || curve == null)
        {
            Debug.LogWarning("Curve or Transform is null.");
            return;
        }

        sq?.Kill(true);

        float t = 0f;
        sq = DOTween.Sequence().Pause();
        sq.Append(DOTween.To(() => t, x =>
        {
            t = x;
            float curveValue = curve.Evaluate(t);
            float remappedScaleFactor = Mathf.LerpUnclamped(fromScale, toScale, curveValue);
            effectTransform.localScale = originalScale * remappedScaleFactor;

        }, 1f, time)
        .SetEase(Ease.Linear)).OnComplete(() =>
        {
            up = false;
        });

        sq.Play();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (button != null && (!button.IsActive() || !button.IsInteractable()))
            return;

        PlayPressDownAnimation();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (button != null && (!button.IsActive() || !button.IsInteractable()))
            return;

        up = true;
        PlayPressExitAnimation();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (button != null && (!button.IsActive() || !button.IsInteractable()))
            return;

        if (up)
            return;

        PlayPressExitAnimation();
    }
}