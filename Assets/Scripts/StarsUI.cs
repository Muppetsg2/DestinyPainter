using UnityEngine;
using UnityEngine.UI;

public class StarsUI : MonoBehaviour
{
    public Image star1Image;
    public Image star2Image;
    public Image star3Image;

    public Shadow star1Shadow;
    public Shadow star2Shadow;
    public Shadow star3Shadow;

    void Update()
    {
        if (LevelManager.Instance.currentStars == 3)
        {
            star3Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, true);
            star2Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, true);
            star1Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, true);

            star3Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, true);
            star2Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, true);
            star1Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, true);
        }
        else if (LevelManager.Instance.currentStars == 2)
        {
            star3Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, false);
            star2Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, true);
            star1Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, true);

            star3Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, false);
            star2Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, true);
            star1Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, true);
        }
        else if (LevelManager.Instance.currentStars == 1)
        {
            star3Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, false);
            star2Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, false);
            star1Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, true);

            star3Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, false);
            star2Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, false);
            star1Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, true);
        }
        else if (LevelManager.Instance.currentStars == 0)
        {
            star3Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, false);
            star2Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, false);
            star1Image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, false);

            star3Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, false);
            star2Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, false);
            star1Shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, false);
        }
    }
}
