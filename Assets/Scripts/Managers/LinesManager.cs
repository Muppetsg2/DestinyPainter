using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class LinesManager : MonoBehaviour
{
    public static LinesManager Instance { get; private set; }

    [System.Serializable]
    public struct LineData
    {
        public Transform planetA;
        public Transform planetB;
        public GameObject line;
        public int jumpCount;

        public bool HasPlanets(Transform a, Transform b)
        {
            return (planetA == a && planetB == b) || (planetA == b && planetB == a);
        }

        public void AddJumpCount()
        {
            float yScaleOriginal = line.transform.localScale.y / jumpCount;
            ++jumpCount;
            if (jumpCount <= 5)
            {
                line.transform.DOScaleY(yScaleOriginal * jumpCount, 0.2f);
            }
        }
    }

    public GameObject linePrefab;
    public List<LineData> lines;

    [Header("Animation")]
    public AnimationCurve lineAnimationCurve;
    public float lineAnimationTime;

    private void Awake()
    {
        if (Instance != null) Destroy(this);
        Instance = this;
    }

    public void AddLine(Transform planetA, Transform planetB)
    {
        int index = lines.FindIndex(lineData => lineData.HasPlanets(planetA, planetB));
        if (index >= 0)
        {
            LineData data = lines[index];
            data.AddJumpCount();
            lines[index] = data;
            return;
        }

        GameObject line = Instantiate(linePrefab, null, true);
        line.name = planetA.name + " -> " + planetB.name;
        line.transform.parent = transform;

        // Scale
        float desiredScaleX = Vector3.Distance(planetA.position, planetB.position);
        line.transform.localScale = new Vector3(0f, line.transform.localScale.y, 1f);

        // Position
        Vector3 desiredPosition = (planetA.position + planetB.position) * 0.5f;
        line.transform.position = planetA.position;

        // Rotation
        Vector3 direction = (planetA.position - planetB.position).normalized;

        float lineAngle = Vector3.Angle(Vector3.right, direction);
        float lineAngleSign = Mathf.Sign(Vector3.Dot(Vector3.forward, Vector3.Cross(Vector3.right, direction)));
        
        line.transform.eulerAngles = new (0f, 0f, lineAngleSign * lineAngle);

        line.transform.DOScaleX(desiredScaleX, lineAnimationTime).SetEase(lineAnimationCurve);
        line.transform.DOMove(desiredPosition, lineAnimationTime).SetEase(lineAnimationCurve);

        lines.Add(new LineData { planetA = planetA, planetB = planetB, line = line, jumpCount = 1 });
    }
}
