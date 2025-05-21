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
            spriteRenderer.material.SetColor("_Color", ColorsManager.Instance.GetSecondaryColor(colorType));
            Vector2 noiseOffset = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
            spriteRenderer.material.SetVector("_NoiseOffset", noiseOffset);

            var lineGenerator = splash.GetComponent<SplashLinesGenerator>();
            lineGenerator.SetColor(colorType);
            lineGenerator.GenerateLines();
            Destroy(gameObject);
        }
    }
}
