using System.Data;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndCanvasScript : MonoBehaviour
{
    public PlayerController player;

    [Header("Visual")]
    public TextMeshProUGUI title;
    public Image backgroung;

    [Header("Buttons")]
    public Button menuBtn;
    public Button shareBtn;
    public Button nextLevelBtn;

    [Header("Stars")]
    public StarObject star1;
    public StarObject star2;
    public StarObject star3;
    [TextArea] public string finishLevelText;
    [TextArea] public string starJumpsTemplate;

    [Header("Other")]
    public TextMeshProUGUI jumpsText;
    public Image levelOverview;
    [TextArea] public string jumpsTemplate;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    void Start()
    {
        LevelManager.Instance.OnStarsChanged.AddListener(UpdateStars);
        star1.SetStarText(finishLevelText);
        star2.SetStarText(ProcessTemplate(starJumpsTemplate, LevelManager.Instance.data.secondStarMaxJumps));
        star3.SetStarText(ProcessTemplate(starJumpsTemplate, LevelManager.Instance.data.thirdStarMaxJumps));

        UpdateStars(LevelManager.Instance.currentStars);

        player.OnJump.AddListener(UpdateJumps);
        UpdateJumps(player.planetJumpsCounter);

        menuBtn.onClick.AddListener(LevelManager.Instance.GoToMenu);
        //shareBtn.onClick.AddListener();
        if (LevelManager.Instance.HasNextLevel()) nextLevelBtn.onClick.AddListener(LevelManager.Instance.GoToNextLevel);
        else nextLevelBtn.gameObject.SetActive(false);
    }

    string ProcessTemplate<T>(string template, T value)
    {
        int numStart = template.IndexOf('{');
        int numEnd = template.IndexOf('}');
        return template[..numStart] + value.ToString() + template.Substring(numEnd + 1, template.Length - numEnd - 1);
    }

    void UpdateJumps(uint jumps)
    {
        jumpsText.text = ProcessTemplate(jumpsTemplate, jumps);
    }

    void UpdateStars(int stars)
    {
        star3.SetStarActive(stars == 3);
        star2.SetStarActive(stars >= 2);
        star1.SetStarActive(stars >= 1);
    }
}
