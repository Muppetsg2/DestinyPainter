using UnityEngine;
using DG.Tweening;

public class SplashSpawner : MonoBehaviour
{
    private static SplashSpawner instance;
    public static SplashSpawner Instance { get { return instance; } }

    [Header("Planet Splash Settings")]
    public GameObject planetSplashPrefab;
    public AnimationCurve planetSplashCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Range(0.0f, 1.0f)] public float planetSplashAlpha = 0.4f;

    [Header("Color Burst Settings")]
    public GameObject colorLinePrefab;
    public int linesCount = 20;
    [Range(0.0f, 180.0f)] public float excludeBackwardAngle = 90.0f;
    [Range(0f, 1f)] public float asymmetryFactor = 0.5f;
    public MinMaxValue<float> pointOffset = new() 
    {
        Min = 0.3f,
        Max = 0.7f
    };
    public MinMaxValue<float> moveDistance = new()
    {
        Min = 1.0f,
        Max = 2.0f
    };
    public float duration = 0.5f;

    private readonly System.Random rng = new();

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Another instance of SplashSpawner exists");
            DestroyImmediate(this);
        }

        instance = this;
    }

    private float MonteCarloRandom(float bias)
    {
        float r = (float)rng.NextDouble();
        return Mathf.Pow(r, bias);
    }

    public void ColorHitBurst(Color primary, Color secondary, Vector3 impactPoint, Vector3 impactNormal, Vector3 impactDirection, float includeAngle = 0f, bool withoutExclusion = false)
    {
        impactNormal.z = 0f;
        impactNormal.Normalize();
        impactNormal = -impactNormal;

        Vector3 ortho = new Vector3(-impactNormal.y, impactNormal.x, 0f);

        impactDirection.z = 0f;
        impactDirection.Normalize();

        float dot = Vector3.Dot(impactDirection, -impactNormal);
        float angleBetween = Mathf.Acos(Mathf.Clamp(dot, -1f, 1f)) * Mathf.Rad2Deg;
        float halfAngleBetween = angleBetween * 0.5f;

        float side = Vector3.Dot(impactDirection, ortho);
        float angleOffset = (side >= 0f) ? halfAngleBetween : -halfAngleBetween;

        int attempts = 0;
        int maxAttempts = linesCount * 5;
        int placed = 0;

        float halfIncludedAngles = 90f + Mathf.Clamp(includeAngle, 0f, 90f);
        float halfExcludedAngles = withoutExclusion ? 0f : excludeBackwardAngle * 0.5f;

        while (placed < linesCount && attempts < maxAttempts)
        {
            attempts++;

            float angle = (MonteCarloRandom(1.0f) * halfIncludedAngles * 2f) - halfIncludedAngles;

            if (angle >= 0f && Mathf.Abs(angle) < halfExcludedAngles - angleOffset * (side >= 0f ? asymmetryFactor : 1f))
                continue;
            if (angle < 0f && Mathf.Abs(angle) < halfExcludedAngles + angleOffset * (side >= 0f ? 1f : asymmetryFactor))
                continue;

            float rad = angle * Mathf.Deg2Rad;
            Vector3 dir = Mathf.Cos(rad) * impactNormal + Mathf.Sin(rad) * ortho;

            float pointOffsetValue = pointOffset.Min + MonteCarloRandom(1.5f) * (pointOffset.Max - pointOffset.Min);
            Vector3 startPos = impactPoint + dir * pointOffsetValue;
            float moveDistanceValue = moveDistance.Min + MonteCarloRandom(1.5f) * (moveDistance.Max - moveDistance.Min);
            Vector3 endPos = impactPoint + dir * (pointOffsetValue + moveDistanceValue);
            float widthMul = moveDistanceValue.Remap(0.0f, moveDistance.Max, 0.0f, 1.0f);

            GameObject lineObj = Instantiate(colorLinePrefab);
            ColorBurstLine line = lineObj.GetComponent<ColorBurstLine>();
            line.PlayAnimation(startPos, endPos, secondary, primary, duration, widthMul);

            placed++;
        }
    }

    public void PlanetSplash(Vector3 c, Color secondary, Vector3 scaleStart, Vector3 scaleEnd, float scaleDur)
    {
        GameObject obj = Instantiate(planetSplashPrefab, c, Quaternion.identity);
        obj.name = "PlanetSplash";

        // Dodaj kolor
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        Color col = secondary;
        col.a = planetSplashAlpha;
        sr.material.SetColor("_Color", col);
        sr.material.SetVector("_NoiseOffset", new Vector2((MonteCarloRandom(1.0f) - 0.5f) * 6.0f, (MonteCarloRandom(1.0f) - 0.5f) * 6.0f));

        // Losowa rotacja Z
        // TODO: na razie linie nie uwzglêdniaj¹ obrotów
        //float randomRotation = (float)rng.NextDouble() * 180f;
        //obj.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

        // Skalowanie z AnimationCurve
        obj.transform.localScale = scaleStart;
        DOTween.To(() => 0f, t => {
            float curveVal = planetSplashCurve.Evaluate(t);
            obj.transform.localScale = Vector3.Lerp(scaleStart, scaleEnd, curveVal);
        }, 1f, scaleDur).OnComplete(() =>
        {
            SplashLinesGenerator slg = obj.GetComponent<SplashLinesGenerator>();
            slg.SetColor(col);
            obj.GetComponent<SplashLinesGenerator>().GenerateLines(col.a * 0.9f);
        });
    }
}