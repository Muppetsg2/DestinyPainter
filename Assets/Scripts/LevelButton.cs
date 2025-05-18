using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour
{
    public ColorsManager colors;
    public MenuManager menu;
    public LevelData data;

    [Header("Lock")]
    public LevelData previousLevel;
    public GameObject lockImage;
    public GameObject levelNameText;
    public GameObject starsHandler;

    [Header("Stars")]
    public Image star1;
    public Image star2;
    public Image star3;

    private void Start()
    {
        if (!IsUnlocked())
        {
            GetComponent<Button>().enabled = false;
            lockImage.SetActive(true);
            levelNameText.SetActive(false);
            starsHandler.SetActive(false);
        }
        else
        {
            GetComponent<Button>().enabled = true;
            lockImage.SetActive(false);
            levelNameText.SetActive(true);
            starsHandler.SetActive(true);

            int starsNum = data == null ? 0 : data.GetLevelStars();
            if (starsNum > 2)
            {
                star3.color = colors.GetStarGetColor();
            }
            else
            {
                star3.color = colors.GetStarDefaultColor();
            }
            if (starsNum > 1)
            {
                star2.color = colors.GetStarGetColor();
            }
            else
            {
                star2.color = colors.GetStarDefaultColor();
            }
            if (starsNum > 0)
            {
                star1.color = colors.GetStarGetColor();
            }
            else
            {
                star1.color = colors.GetStarDefaultColor();
            }
        }
    }

    public bool IsUnlocked()
    {
        return previousLevel == null || previousLevel.GetLevelFinished();
    }

    public void OpenLevel()
    {
        data.LoadLevel();
    }
}
