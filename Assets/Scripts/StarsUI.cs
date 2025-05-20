using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StarsUI : MonoBehaviour
{
    public LevelManager manager;
    public ColorsManager colors;

    public Image star1;
    public Image star2;
    public Image star3;

    public TextMeshProUGUI starMsgText;

    public string maxJumpsMsg;
    public string finishLevelMsg;

    void Update()
    {
        if (manager.currentStars == 3)
        {
            star3.color = colors.GetStarColor();
            star2.color = colors.GetStarColor();
            star1.color = colors.GetStarColor();

            starMsgText.text = maxJumpsMsg[..maxJumpsMsg.IndexOf('{')] + manager.data.thirdStarMaxJumps + maxJumpsMsg.Substring(maxJumpsMsg.IndexOf('}') + 1, maxJumpsMsg.Length - maxJumpsMsg.IndexOf('}') - 1);
        }
        else if (manager.currentStars == 2)
        {
            star3.color = colors.GetStarDefaultColor();
            star2.color = colors.GetStarColor();
            star1.color = colors.GetStarColor();

            starMsgText.text = maxJumpsMsg[..maxJumpsMsg.IndexOf('{')] + manager.data.secondStarMaxJumps + maxJumpsMsg.Substring(maxJumpsMsg.IndexOf('}') + 1, maxJumpsMsg.Length - maxJumpsMsg.IndexOf('}') - 1);
        }
        else if (manager.currentStars == 1)
        {
            star3.color = colors.GetStarDefaultColor();
            star2.color = colors.GetStarDefaultColor();
            star1.color = colors.GetStarColor();

            starMsgText.text = finishLevelMsg;
        }
        else if (manager.currentStars == 0)
        {
            star3.color = colors.GetStarDefaultColor();
            star2.color = colors.GetStarDefaultColor();
            star1.color = colors.GetStarDefaultColor();

            starMsgText.text = finishLevelMsg;
        }
    }
}
