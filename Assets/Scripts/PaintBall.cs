using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PaintBall : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public GameObject splashPrefab;

    public ColorType colorType;
    public Vector3 scale;
    public bool playAudio;

    public PaintBallSpawner spawner;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetColor(ColorType color)
    {
        colorType = color;
        meshRenderer.material.SetColor("_Center", ColorsManager.Instance.GetColor(color, ColorCategory.Primary));
        meshRenderer.material.SetColor("_Outer", ColorsManager.Instance.GetColor(color, ColorCategory.Secondary));
    }

    public void SetScale(Vector3 value)
    {
        scale = value;
    }

    public void SetPlayAudio(bool value)
    {
        playAudio = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            GameObject splash = Instantiate(splashPrefab, transform.position, Quaternion.identity);
            splash.transform.localScale = scale;
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

            if (playAudio) PlayAudio();

            Destroy(gameObject);
        }
    }

    private void PlayAudio()
    {
        GameObject aus = new GameObject("AudioSource (" + gameObject.name + ")");
        AudioSource s = aus.AddComponent<AudioSource>();
        s.playOnAwake = audioSource.playOnAwake;
        s.mute = audioSource.mute;
        s.volume = audioSource.volume;
        s.loop = audioSource.loop;
        s.clip = audioSource.clip;

        s.Play();
        Destroy(aus, 1f);
    }
}
