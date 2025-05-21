using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance { get; private set; }

    [Header("Animation")]
    public RectTransform canvas;
    public RectTransform circleMask;
    public GameObject panel;
    public AnimationCurve animCurve;
    public float animTime = 2f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadLevel(string sceneName)
    {
        circleMask.gameObject.SetActive(true);
        panel.SetActive(true);

        float width = 2f * canvas.sizeDelta.magnitude;
        circleMask.sizeDelta = new Vector2(width, width);
        circleMask.DOSizeDelta(Vector2.zero, animTime).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        circleMask.gameObject.SetActive(true);
        panel.SetActive(true);

        float width = 2f * canvas.sizeDelta.magnitude;
        circleMask.sizeDelta = Vector2.zero;
        circleMask.DOSizeDelta(new Vector2(width, width), animTime).OnComplete(() =>
        {
            circleMask.gameObject.SetActive(false);
            panel.SetActive(false);
        });
    }
}
