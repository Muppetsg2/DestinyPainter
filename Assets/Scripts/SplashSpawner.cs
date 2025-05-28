using UnityEngine;
using DG.Tweening;
using SaintsField;
using SaintsField.Playa;

public class SplashSpawner : MonoBehaviour
{
    private static SplashSpawner instance;
    public static SplashSpawner Instance { get { return instance; } }

    public GameObject planetSplashPrefab;

    [Header("Sprite Renderer Settings")]
    public string sortingLayerName = "Default";
    public int sortingOrder = 0;

    [Header("Splash Settings")]
    public bool useNewSplash = false;

    [HideIf(nameof(useNewSplash))]
    public Sprite spriteOld;

    [ShowIf(nameof(useNewSplash))]
    public Sprite spriteNew;

    public int splashesCount = 8;
    public float spawnRadius = 0.5f;

    [Header("Splash Test Settings")]
    public Vector3 splashTestCenter = Vector3.zero;

    [Header("Splash Colors")]
    public Color colorFrom = Color.white;
    public Color colorTo = Color.red;
    [HideIf(nameof(useNewSplash))]
    public Material baseMaterialOld;
    [ShowIf(nameof(useNewSplash))]
    public Material baseMaterialNew;

    [Header("Splash Scale Animation")]
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float scaleDuration = 0.5f;
    public Vector3 startScale = Vector3.one * 0.5f;
    public Vector3 targetScale = Vector3.one;

    [Header("Planet Splash Animation")]
    public AnimationCurve planetSplashCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float planetSplashAlpha = 0.4f;

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

    private Transform spawnContainer;
    private float goldenAngle = 137.508f;
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

    private Material CreateMaterial(Color from, Color to)
    {
        Material mat = new Material(useNewSplash ? baseMaterialNew : baseMaterialOld);
        mat.SetColor("_Color", Color.Lerp(from, to, MonteCarloRandom(1.5f)));
        if(useNewSplash)
        {
            mat.SetVector("_NoiseOffset", new Vector2((MonteCarloRandom(1.0f) - 0.5f) * 6.0f, (MonteCarloRandom(1.0f) - 0.5f) * 6.0f));
        }
        else
        {
            mat.SetVector("_Magnitude", new Vector2((MonteCarloRandom(1.0f) - 0.5f) * 0.25f, (MonteCarloRandom(1.0f) - 0.5f) * 0.25f));
        }
        return mat;
    }

    [Button]
    [ContextMenu("Delete Splashes")]
    public void DeleteSplashes()
    {
        if (spawnContainer == null)
        {
            GameObject containerObj = new GameObject("SpawnedSplashes");
            containerObj.transform.SetParent(transform);
            spawnContainer = containerObj.transform;
        }

        for (int i = spawnContainer.childCount - 1; i >= 0; --i)
        {
            DestroyImmediate(spawnContainer.GetChild(i).gameObject);
        }

        DestroyImmediate(spawnContainer);
    }

    [Button]
    [ContextMenu("Spawn Splashes")]
    public void SpawnSplashes()
    {
        DeleteSplashes();

        for (int i = 0; i < splashesCount; ++i)
        {
            GameObject obj = new GameObject("Splash_" + i);
            obj.transform.SetParent(spawnContainer);
            obj.transform.position = splashTestCenter;

            // Dodaj sprite i kolor
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = useNewSplash ? spriteNew : spriteOld;
            //sr.color = Color.Lerp(colorFrom, colorTo, (float)rng.NextDouble());
            sr.color = Color.white;
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder;
            sr.material = CreateMaterial(colorFrom, colorTo);

            // Losowa rotacja Z
            float randomRotation = (float)rng.NextDouble() * 180f;
            obj.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

            // Losowe przesuniêcie od œrodka
            float angle = i * goldenAngle * Mathf.Deg2Rad;
            //float radius = Mathf.Lerp(0.1f, spawnRadius, Mathf.Sqrt((float)i / count));
            float radius = Mathf.Lerp(0.1f, spawnRadius, MonteCarloRandom(2.0f));
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Vector3 targetPos = splashTestCenter + new Vector3(offset.x, offset.y, 0f);
            obj.transform.DOMove(targetPos, 0.4f).SetEase(Ease.OutQuad);

            // Skalowanie z AnimationCurve
            obj.transform.localScale = startScale;
            DOTween.To(() => 0f, t => {
                float curveVal = scaleCurve.Evaluate(t);
                obj.transform.localScale = Vector3.Lerp(startScale, targetScale, curveVal);
            }, 1f, scaleDuration);
        }
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

    public void PlayerDeathSplashes(Vector3 c, Color primary, Color secondary, float alpha = 1.0f)
    {
        for (int i = 0; i < splashesCount; ++i)
        {
            GameObject obj = new GameObject("SplashPlayerDeath_" + i);
            obj.transform.SetParent(spawnContainer);
            obj.transform.position = c;

            // Dodaj sprite i kolor
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = useNewSplash ? spriteNew : spriteOld;
            sr.color = Color.white;
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder;

            Material mat = new Material(baseMaterialOld);
            primary.a = 0.7058824f;
            secondary.a = 0.7058824f;
            sr.material = CreateMaterial(primary, secondary);

            // Losowa rotacja Z
            float randomRotation = (float)rng.NextDouble() * 180f;
            obj.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

            // Losowe przesuniêcie od œrodka
            float angle = i * goldenAngle * Mathf.Deg2Rad;
            //float radius = Mathf.Lerp(0.1f, spawnRadius, Mathf.Sqrt((float)i / count));
            float radius = Mathf.Lerp(0.1f, spawnRadius, MonteCarloRandom(2.0f));
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Vector3 targetPos = c + new Vector3(offset.x, offset.y, 0f);
            obj.transform.DOMove(targetPos, 0.4f).SetEase(Ease.OutQuad);

            // Skalowanie z AnimationCurve
            obj.transform.localScale = startScale;
            DOTween.To(() => 0f, t => {
                float curveVal = scaleCurve.Evaluate(t);
                obj.transform.localScale = Vector3.Lerp(startScale, targetScale, curveVal);
            }, 1f, scaleDuration);
        }
    }

    public void PlanetSplash(Vector3 c, Color secondary, Vector3 scaleStart, Vector3 scaleEnd, float scaleDur)
    {
        GameObject obj = Instantiate(planetSplashPrefab, c, Quaternion.identity);
        obj.name = "PlanetSplash";

        // Dodaj sprite i kolor
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        sr.sprite = useNewSplash ? spriteNew : spriteOld;
        Color col = Color.white;
        col.a = planetSplashAlpha;
        sr.color = col;
        sr.sortingLayerName = sortingLayerName;
        sr.sortingOrder = sortingOrder;
        col = Color.Lerp(secondary, secondary, MonteCarloRandom(1.5f));
        sr.material.SetColor("_Color", col);
        sr.material.SetVector("_NoiseOffset", new Vector2((MonteCarloRandom(1.0f) - 0.5f) * 6.0f, (MonteCarloRandom(1.0f) - 0.5f) * 6.0f));
        //sr.material = CreateMaterial(secondary, secondary);

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
            obj.GetComponent<SplashLinesGenerator>().GenerateLines(col.a);
        });
    }
}