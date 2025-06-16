using DG.Tweening;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(AudioSource))]
public class ButtonClickBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [System.Serializable]
    public enum IdleColorMode { Single, Gradient, List }

    [Header("Audio Settings")]
    [SerializeField] private bool useAudio = true;
    [ShowIf(nameof(useAudio)), SerializeField] private bool forceMode = false;

    [Header("Animation Settings")]
    [SerializeField] private Transform effectTransform;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float pressedScaleFactor = 0.5f;
    [SerializeField] private float time = 0.2f;

    [Header("Idle Animation Settings")]
    [SerializeField] private bool idleAnimation = false;

    [LayoutShowIf(nameof(idleAnimation))]
    [LayoutStart("Idle Scale Animation Settings", ELayout.FoldoutBox)]
    [ShowIf(nameof(idleAnimation)), SerializeField] private bool idleScaleAnimation = true;
    [ShowIf(nameof(idleAnimation), nameof(idleScaleAnimation)), SerializeField] private float idleScaleValue = 1.1f;
    [ShowIf(nameof(idleAnimation), nameof(idleScaleAnimation)), SerializeField] private float idleScaleDuration = 1.75f;
    [ShowIf(nameof(idleAnimation), nameof(idleScaleAnimation)), SerializeField] private LoopType idleScaleLoopType = LoopType.Yoyo;
    [LayoutEnd(".")]

    [LayoutShowIf(nameof(idleAnimation))]
    [LayoutStart("Idle Color Animation Settings", ELayout.FoldoutBox)]
    [ShowIf(nameof(idleAnimation)), SerializeField] private bool idleColorAnimation = true;
    [ShowIf(nameof(idleAnimation), nameof(idleColorAnimation)), SerializeField] private Image image;
    [ShowIf(nameof(idleAnimation), nameof(idleColorAnimation)), SerializeField] private float idleColorDuration = 1.75f;
    [ShowIf(nameof(idleAnimation), nameof(idleColorAnimation)), SerializeField] private LoopType idleColorLoopType = LoopType.Yoyo;
    [ShowIf(nameof(idleAnimation), nameof(idleColorAnimation)), SerializeField] private IdleColorMode colorMode = IdleColorMode.Single;
    [ShowIf(nameof(idleAnimation), nameof(idleColorAnimation), nameof(colorMode), IdleColorMode.Single), SerializeField] private Color idleColor = Color.yellow;
    [ShowIf(nameof(idleAnimation), nameof(idleColorAnimation), nameof(colorMode), IdleColorMode.Gradient), SerializeField] private Gradient idleGradient;
    [ShowIf(nameof(idleAnimation), nameof(idleColorAnimation), nameof(colorMode), IdleColorMode.List), SerializeField] private SaintsList<Color> idleColorsList;
    [LayoutEnd(".")]

    // Audio
    private AudioSource audioSource;

    // Animation
    private Button button;
    private Vector3 originalScale;
    private Sequence sq = null;
    private bool up = false;

    // Idle Animation
    private Color originalColor;
    private Tween idleTween = null;
    private Tween colorTween = null;
    private bool down = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (effectTransform != null) 
            originalScale = effectTransform.localScale;

        if (image != null)
            originalColor = image.color;

        if (idleAnimation)
            StartIdleAnimation();

        button.onClick.AddListener(() => 
        {
            StopIdleAnimation();
            if (useAudio)
            {
                if (!forceMode)
                {
                    audioSource.Play();
                    return;
                }

                GameObject aus = new GameObject("AudioSourceForce (" + gameObject.name + ")");
                AudioSource s = aus.AddComponent<AudioSource>();
                s.playOnAwake = audioSource.playOnAwake;
                s.mute = audioSource.mute;
                s.volume = audioSource.volume;
                s.loop = audioSource.loop;
                s.clip = audioSource.clip;

                s.Play();
                Destroy(audioSource, 0.5f);
            }
        });
    }

    private void OnDestroy()
    {
        sq?.Kill(true);
        StopIdleAnimation();
    }

    private void StartIdleAnimation()
    {
        if (effectTransform == null)
            return;

        StopIdleAnimation(); // Ensure only one is running

        if (idleScaleAnimation)
        {
            idleTween = effectTransform
                .DOScale(originalScale * idleScaleValue, idleScaleDuration * 0.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, idleScaleLoopType);
        }

        if (image != null && idleColorAnimation)
        {
            if (colorMode == IdleColorMode.Single)
            {
                colorTween = image.DOColor(idleColor, idleColorDuration * 0.5f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, idleColorLoopType);
            }
            else if (colorMode == IdleColorMode.Gradient || colorMode == IdleColorMode.List)
            {
                AnimateDiffrentColor();
            }
        }
    }

    private void StopIdleAnimation()
    {
        idleTween?.Kill(true);
        colorTween?.Kill(true);
        idleTween = null;
        colorTween = null;

        if (effectTransform != null)
            effectTransform.localScale = originalScale;

        if (image != null)
        {
            DOTween.Kill(image, true);
            image.color = originalColor;
        }
    }

    private void AnimateDiffrentColor()
    {
        Color nextColor = Color.white;
        if (colorMode == IdleColorMode.List && idleColorsList.Count > 0)
        {
            nextColor = idleColorsList[Random.Range(0, idleColorsList.Count)];
        }
        else if (colorMode == IdleColorMode.Gradient)
        {
            nextColor = idleGradient.Evaluate(Random.Range(0f, 1f));
        }

        image.DOColor(nextColor, idleColorDuration * 0.5f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                // Back to original, then loop again
                image.DOColor(originalColor, idleColorDuration * 0.5f)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        AnimateDiffrentColor(); // recursive call
                    });
            });
    }

    private void PlayScaleAnimation(float fromScale, float toScale)
    {
        if (effectTransform == null || curve == null)
        {
            Debug.LogWarning("Curve or Transform is null.");
            return;
        }

        sq?.Kill(true);
        StopIdleAnimation();

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

            if (!down && idleAnimation)
            {
                StartIdleAnimation();
            }
        });

        sq.Play();
    }

    public void PlayPressDownAnimation()
    {
        PlayScaleAnimation(originalScale.x, pressedScaleFactor);
    }

    public void PlayPressExitAnimation()
    {
        PlayScaleAnimation(effectTransform.localScale.x, originalScale.x);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (button != null && (!button.IsActive() || !button.IsInteractable()))
            return;

        down = true;
        PlayPressDownAnimation();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (button != null && (!button.IsActive() || !button.IsInteractable()))
            return;

        down = false;
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