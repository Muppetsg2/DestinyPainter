using UnityEngine;

public class PaintBall : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public GameObject splashPrefab;

    public ColorType colorType;

    public void SetColor(ColorType color)
    {
        colorType = color;
        meshRenderer.material.SetColor("_Center", ColorsManager.Instance.GetPrimaryColor(color));
        meshRenderer.material.SetColor("_Outer", ColorsManager.Instance.GetSecondaryColor(color));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            GameObject splash = Instantiate(splashPrefab, transform.position, Quaternion.identity);

            var spriteRenderer = splash.GetComponent<SpriteRenderer>();
            Color color = ColorsManager.Instance.GetSecondaryColor(colorType);
            color.a = 150f / 255f;
            spriteRenderer.material.SetColor("_Color", color);
            Vector2 noiseOffset = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
            spriteRenderer.material.SetVector("_NoiseOffset", noiseOffset);

            var lineGenerator = splash.GetComponent<SplashLinesGenerator>();
            lineGenerator.SetColor(colorType);
            lineGenerator.GenerateLines(150f / 255f);
            Destroy(gameObject);
        }
    }
}
