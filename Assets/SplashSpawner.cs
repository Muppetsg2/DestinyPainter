using UnityEngine;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SplashSpawner : MonoBehaviour
{
    private SplashSpawner instance;
    public SplashSpawner Instance { get { return instance; } }

    [Header("Main Settings")]
    public Sprite sprite;
    public int count = 5;
    public Vector3 center = Vector3.zero;
    public float spawnRadius = 0.5f;

    [Header("Sprite Renderer Settings")]
    public string sortingLayerName = "Default";
    public int sortingOrder = 0;

    [Header("Splash Scale Animation")]
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float scaleDuration = 0.5f;
    public Vector3 startScale = Vector3.one * 0.5f;
    public Vector3 targetScale = Vector3.one;

    [Header("Colors")]
    public Color colorFrom = Color.white;
    public Color colorTo = Color.red;
    public Material baseMaterial;

    private Transform spawnContainer;
    private float goldenAngle = 137.508f;
    private System.Random rng = new System.Random();

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
    }

    [ContextMenu("Spawn Splashes")]
    public void SpawnSplashes()
    {
        DeleteSplashes();

        for (int i = 0; i < count; ++i)
        {
            GameObject obj = new GameObject("Splash_" + i);
            obj.transform.SetParent(spawnContainer);
            obj.transform.position = center;

            // Dodaj sprite i kolor
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            //sr.color = Color.Lerp(colorFrom, colorTo, (float)rng.NextDouble());
            sr.color = Color.Lerp(colorFrom, colorTo, MonteCarloRandom(1.5f));
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder;

            Material mat = new Material(baseMaterial);
            mat.SetColor("_Color", Color.white);
            //mat.SetVector("_Magnitude", new Vector2(((float)rng.NextDouble() - 0.5f) * 0.25f, ((float)rng.NextDouble() - 0.5f) * 0.25f));
            mat.SetVector("_Magnitude", new Vector2((MonteCarloRandom(1.0f) - 0.5f) * 0.25f, (MonteCarloRandom(1.0f) - 0.5f) * 0.25f));
            sr.material = mat;

            // Losowa rotacja Z
            float randomRotation = (float)rng.NextDouble() * 180f;
            obj.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

            // Losowe przesuniêcie od œrodka
            float angle = i * goldenAngle * Mathf.Deg2Rad;
            //float radius = Mathf.Lerp(0.1f, spawnRadius, Mathf.Sqrt((float)i / count));
            float radius = Mathf.Lerp(0.1f, spawnRadius, MonteCarloRandom(2.0f));
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Vector3 targetPos = center + new Vector3(offset.x, offset.y, 0f);
            obj.transform.DOMove(targetPos, 0.4f).SetEase(Ease.OutQuad);

            // Skalowanie z AnimationCurve
            obj.transform.localScale = startScale;
            DOTween.To(() => 0f, t => {
                float curveVal = scaleCurve.Evaluate(t);
                obj.transform.localScale = Vector3.Lerp(startScale, targetScale, curveVal);
            }, 1f, scaleDuration);
        }
    }

    public void PlayerDeathSplashes(Vector3 c, Color primary, Color secondary)
    {
        for (int i = 0; i < count; ++i)
        {
            GameObject obj = new GameObject("SplashPlayerDeath_" + i);
            obj.transform.SetParent(spawnContainer);
            obj.transform.position = c;

            // Dodaj sprite i kolor
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = Color.Lerp(primary, secondary, MonteCarloRandom(1.5f));
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder;

            Material mat = new Material(baseMaterial);
            mat.SetColor("_Color", Color.white);
            //mat.SetVector("_Magnitude", new Vector2(((float)rng.NextDouble() - 0.5f) * 0.25f, ((float)rng.NextDouble() - 0.5f) * 0.25f));
            mat.SetVector("_Magnitude", new Vector2((MonteCarloRandom(1.0f) - 0.5f) * 0.25f, (MonteCarloRandom(1.0f) - 0.5f) * 0.25f));
            sr.material = mat;

            // Losowa rotacja Z
            float randomRotation = (float)rng.NextDouble() * 180f;
            obj.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

            // Losowe przesuniêcie od œrodka
            float angle = i * goldenAngle * Mathf.Deg2Rad;
            //float radius = Mathf.Lerp(0.1f, spawnRadius, Mathf.Sqrt((float)i / count));
            float radius = Mathf.Lerp(0.1f, spawnRadius, MonteCarloRandom(2.0f));
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Vector3 targetPos = center + new Vector3(offset.x, offset.y, 0f);
            obj.transform.DOMove(targetPos, 0.4f).SetEase(Ease.OutQuad);

            // Skalowanie z AnimationCurve
            obj.transform.localScale = startScale;
            DOTween.To(() => 0f, t => {
                float curveVal = scaleCurve.Evaluate(t);
                obj.transform.localScale = Vector3.Lerp(startScale, targetScale, curveVal);
            }, 1f, scaleDuration);
        }
    }

    public void PlanetSplash(Vector3 c, Color secondary, Vector3 scaleStart, Vector3 scaleEnd, float scaleDur, float fadeDelay, float fadeDuration)
    {
        GameObject obj = new GameObject("PlanetSplash");
        obj.transform.position = c;

        // Dodaj sprite i kolor
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = secondary;
        sr.sortingLayerName = sortingLayerName;
        sr.sortingOrder = sortingOrder;

        Material mat = new Material(baseMaterial);
        mat.SetColor("_Color", Color.white);
        mat.SetVector("_Magnitude", new Vector2((MonteCarloRandom(1.0f) - 0.5f) * 0.25f, (MonteCarloRandom(1.0f) - 0.5f) * 0.25f));
        sr.material = mat;

        // Losowa rotacja Z
        float randomRotation = (float)rng.NextDouble() * 180f;
        obj.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

        // Skalowanie z AnimationCurve
        obj.transform.localScale = scaleStart;
        DOTween.To(() => 0f, t => {
            float curveVal = scaleCurve.Evaluate(t);
            obj.transform.localScale = Vector3.Lerp(scaleStart, scaleEnd, curveVal);
        }, 1f, scaleDur);

        Color initialColor = sr.color;
        DOVirtual.DelayedCall(fadeDelay, () =>
        {
            sr.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                DestroyImmediate(obj);
            });
        });
    }
}