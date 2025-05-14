using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MenuButton : MonoBehaviour
{
    [Header("Relations")]
    public RectTransform line;
    public RectTransform parentPlanet;

    [Header("Animation")]
    public float minMoveRange = 0.5f;
    public float maxMoveRange = 1f;
    public float minMoveTime = 1f;
    public float maxMoveTime = 2f;
    public AnimationCurve moveCurve;

    private RectTransform selfRect;
    private Vector3 startPosition;

    private void Awake()
    {
        selfRect = GetComponent<RectTransform>();
        startPosition = selfRect.position;
        CalculateLine();
    }

    private void Start()
    {
        CalculateNextMovePoint();
    }

    private void Update()
    {
        CalculateLine();
    }

    void CalculateNextMovePoint()
    {
        float angle = Random.Range(0f, 360f);
        float t = Random.Range(0f, 1f);
        float moveRange = (1f - t) * minMoveRange + t * maxMoveRange;
        Vector3 destination = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * moveRange + startPosition;

        float moveTime = (1f - t) * minMoveTime + t * maxMoveTime;
        selfRect.DOMove(destination, moveTime).SetLoops(0).SetEase(moveCurve).OnComplete(CalculateNextMovePoint);
    }

    [ContextMenu("Recalculate Line")]
    void CalculateLine()
    {
        if (selfRect == null) selfRect = GetComponent<RectTransform>();
        line.sizeDelta = new Vector2(line.sizeDelta.x, Vector2.Distance(selfRect.anchoredPosition, parentPlanet.anchoredPosition));
        line.position = (selfRect.position + parentPlanet.position) / 2f;

        Vector3 direction = (selfRect.anchoredPosition - parentPlanet.anchoredPosition).normalized;

        float lineAngle = Vector3.Angle(Vector3.up, direction);
        float lineAngleSign = Mathf.Sign(Vector3.Dot(Vector3.forward, Vector3.Cross(Vector3.up, direction)));

        line.transform.localEulerAngles = new Vector3(0f, 0f, lineAngleSign * lineAngle);
    }
}
