using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour
{
    public MenuManager menu;
    public LevelData data;

    [Header("Lock")]
    public LevelData previousLevel;
    public GameObject lockImage;
    public GameObject levelNameText;
    public GameObject starsHandler;

    [Header("Stars")]
    public Image star1Image;
    public Image star2Image;
    public Image star3Image;
    public Shadow star1Shadow;
    public Shadow star2Shadow;
    public Shadow star3Shadow;

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
                star3Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, true);
                star3Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, true);
            }
            else
            {
                star3Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, false);
                star3Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, false);
            }
            if (starsNum > 1)
            {
                star2Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, true);
                star2Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, true);
            }
            else
            {
                star2Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, false);
                star2Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, false);
            }
            if (starsNum > 0)
            {
                star1Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, true);
                star1Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, true);
            }
            else
            {
                star1Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, false);
                star1Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, false);
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
