using UnityEngine;

public class PaintBall : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public GameObject splashPrefab;

    public ColorType colorType;

    public PaintBallSpawner spawner;

    public void SetColor(ColorType color)
    {
        colorType = color;
        meshRenderer.material.SetColor("_Center", ColorsManager.Instance.GetColor(color, ColorCategory.Primary));
        meshRenderer.material.SetColor("_Outer", ColorsManager.Instance.GetColor(color, ColorCategory.Secondary));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            GameObject splash = Instantiate(splashPrefab, transform.position, Quaternion.identity);

            var spriteRenderer = splash.GetComponent<SpriteRenderer>();
            Color color = ColorsManager.Instance.GetColor(colorType, ColorCategory.Secondary);
            color.a = 150f / 255f;
            spriteRenderer.material.SetColor("_Color", color);
            Vector2 noiseOffset = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
            spriteRenderer.material.SetVector("_NoiseOffset", noiseOffset);

            var lineGenerator = splash.GetComponent<SplashLinesGenerator>();
            lineGenerator.SetColor(color);
            lineGenerator.GenerateLines(150f / 255f);

            if (spawner != null) spawner.AddSplash(splash);

            Destroy(gameObject);
        }
    }
}
