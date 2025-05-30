using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseCanvasScript : MonoBehaviour
{
    [Header("Buttons")]
    public Button menuBtn;
    public Button restartBtn;
    public Button playBtn;

    [Header("Stars")]
    public Image star1Img;
    public Image star2Img;
    public Image star3Img;
    public Shadow star1Shadow;
    public Shadow star2Shadow;
    public Shadow star3Shadow;

    [Header("Goals")]
    public TextMeshProUGUI star2Reason;
    public TextMeshProUGUI star3Reason;

    void Start()
    {
        
    }

    void Update()
    {
        UpdateStars();
        UpdateGoals();
    }

    void UpdateStars()
    {

    }

    void UpdateGoals()
    {

    }
}
