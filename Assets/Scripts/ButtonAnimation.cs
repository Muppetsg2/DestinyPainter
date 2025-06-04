using DG.Tweening;
using System;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    public Vector3 position;

    [Header("Relations")]
    public RectTransform line;
    public RectTransform parentPlanet;

    [Header("Idle Animation")]
    public float minMoveRange = 0.5f;
    public float maxMoveRange = 1f;
    public float minMoveTime = 1f;
    public float maxMoveTime = 2f;
    public AnimationCurve moveCurve;

    [Header("Hide Animation")]
    public float hideTime = 2f;
    public AnimationCurve hideCurve;

    [Header("Show Animation")]
    public float showTime = 2f;
    public AnimationCurve showCurve;

    private RectTransform selfRect;

    public enum AnimationType
    {
        None, Idle, Hide, Show
    }

    private Tween currentTween;
    public AnimationType AnimType { get; private set; } = AnimationType.None;

    private void Awake()
    {
        selfRect = GetComponent<RectTransform>();
        CalculateLine();
    }

    public void PlayIdle()
    {
        if (AnimType != AnimationType.None) return;
        AnimType = AnimationType.Idle;
        CalculateNextMovePoint();
    }

    public void StopIdle()
    {
        if (AnimType != AnimationType.Idle) return;
        AnimType = AnimationType.None;
        currentTween.Kill();
    }

    public void PlayHide(Action onComplete = null)
    {
        if (AnimType != AnimationType.None) return;
        AnimType = AnimationType.Hide;

        var core = selfRect.DOMove(parentPlanet.position, hideTime).SetEase(hideCurve);
        currentTween = core.OnUpdate(() =>
        {
            if (Mathf.Abs(Vector3.Distance(core.endValue, parentPlanet.position)) > 1f)
            {
                core.ChangeEndValue(parentPlanet.position, hideTime - core.Elapsed(), true);
            }
            CalculateLine();
        }
        ).OnComplete(() =>
        {
            AnimType = AnimationType.None;
            onComplete?.Invoke();
        });
    }

    public void StopHide()
    {
        if (AnimType != AnimationType.Hide) return;
        AnimType = AnimationType.None;
        currentTween.Kill();
    }

    public void PlayShow(bool playIdle = true, Action onComplete = null)
    {
        if (AnimType != AnimationType.None) return;
        AnimType = AnimationType.Show;

        CalculateLine();
        currentTween = selfRect.DOAnchorPos3D(position, showTime).SetEase(showCurve).OnUpdate(CalculateLine).OnComplete(() =>
        {
            AnimType = AnimationType.None;
            if (playIdle) PlayIdle();
            onComplete?.Invoke();
        });
    }

    public void StopShow()
    {
        if (AnimType != AnimationType.Show) return;
        AnimType = AnimationType.None;
        currentTween.Kill();
    }

    void CalculateNextMovePoint()
    {
        float angle = UnityEngine.Random.Range(0f, 360f);
        float t = UnityEngine.Random.Range(0f, 1f);
        float moveRange = (1f - t) * minMoveRange + t * maxMoveRange;
        Vector3 destination = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * moveRange + position;

        float moveTime = (1f - t) * minMoveTime + t * maxMoveTime;
        currentTween = selfRect.DOAnchorPos3D(destination, moveTime).SetEase(moveCurve).OnComplete(CalculateNextMovePoint).OnUpdate(CalculateLine);
    }

    [ContextMenu("Recalculate Line")]
    void CalculateLine()
    {
        float sizeY = 0;
        if (transform.position != parentPlanet.position) sizeY = Vector2.Distance(transform.position, parentPlanet.position);
        line.sizeDelta = new Vector2(line.sizeDelta.x, sizeY / line.lossyScale.y);
        line.position = (transform.position + parentPlanet.position) / 2f;

        Vector3 direction = (transform.position - parentPlanet.position).normalized;

        float lineAngle = Vector3.Angle(Vector3.up, direction);
        float lineAngleSign = Mathf.Sign(Vector3.Dot(Vector3.forward, Vector3.Cross(Vector3.up, direction)));

        line.transform.localEulerAngles = new Vector3(0f, 0f, lineAngleSign * lineAngle);
    }
}
