using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseCanvasScript : MonoBehaviour
{
    [Header("Visual")]
    public TextMeshProUGUI title;
    public Transform stars;
    public Transform buttons;
    public Image background;

    [Header("Buttons")]
    public Button menuBtn;
    public Button restartBtn;
    public Button playBtn;

    [Header("Stars")]
    public StarObject star1;
    public StarObject star2;
    public StarObject star3;
    [TextArea] public string finishLevelText;
    [TextArea] public string starJumpsTemplate;

    [Header("Open Animation")]
    public float backgroundOpenTime = 0.3f;
    public float showItemsTime = 0.3f;
    public float showItemsDelays = 0.1f;
    public AnimationCurve openAnimCurve;

    [Header("Close Animation")]
    public float backgroundCloseTime = 0.3f;
    public float hideItemsTime = 0.3f;
    public float hideItemsDelays = 0.1f;
    public AnimationCurve closeAnimCurve;

    void Start()
    {
        LevelManager.Instance.OnStarsChanged.AddListener(UpdateStars);
        star1.SetStarText(finishLevelText);
        star2.SetStarText(GetStarJumpsText(LevelManager.Instance.data.secondStarMaxJumps));
        star3.SetStarText(GetStarJumpsText(LevelManager.Instance.data.thirdStarMaxJumps));

        UpdateStars(LevelManager.Instance.currentStars);

        menuBtn.onClick.AddListener(LevelManager.Instance.GoToMenu);
        restartBtn.onClick.AddListener(LevelManager.Instance.RestartLevel);
        playBtn.onClick.AddListener(LevelManager.Instance.UnpauseLevel);
    }

    public void Open()
    {
        float bottomY = -GetComponent<RectTransform>().rect.height;

        Color bg = background.color;
        float backgroundOpenAlpha = bg.a;
        bg.a = 0f;
        background.color = bg;

        Vector3 local = title.transform.localPosition;
        float titleLocalY = local.y;
        local.y = bottomY;
        title.transform.localPosition = local;

        local = stars.transform.localPosition;
        float starsLocalY = local.y;
        local.y = bottomY;
        stars.transform.localPosition = local;

        local = buttons.transform.localPosition;
        float buttonsLocalY = local.y;
        local.y = bottomY;
        buttons.transform.localPosition = local;

        menuBtn.enabled = false;
        restartBtn.enabled = false;
        playBtn.enabled = false;

        background.DOFade(backgroundOpenAlpha, backgroundOpenTime).OnComplete(() =>
        {
            title.transform.DOLocalMoveY(titleLocalY, showItemsTime).SetEase(openAnimCurve);
            stars.DOLocalMoveY(starsLocalY, showItemsTime).SetDelay(showItemsDelays).SetEase(openAnimCurve);
            buttons.DOLocalMoveY(buttonsLocalY, showItemsTime).SetDelay(showItemsDelays * 2).SetEase(openAnimCurve).OnComplete(() =>
            {
                menuBtn.enabled = true;
                restartBtn.enabled = true;
                playBtn.enabled = true;
            });
        });
    }

    public void Close(Action onComplete = null)
    {
        float bottomY = -GetComponent<RectTransform>().rect.height;

        float backgroundOpenAlpha = background.color.a;
        float titleLocalY = title.transform.localPosition.y;
        float starsLocalY = stars.transform.localPosition.y;
        float buttonsLocalY = buttons.transform.localPosition.y;

        menuBtn.enabled = false;
        restartBtn.enabled = false;
        playBtn.enabled = false;

        buttons.DOLocalMoveY(bottomY, hideItemsTime).SetEase(closeAnimCurve);
        stars.DOLocalMoveY(bottomY, hideItemsTime).SetDelay(hideItemsDelays).SetEase(closeAnimCurve);
        title.transform.DOLocalMoveY(bottomY, hideItemsTime).SetDelay(hideItemsDelays * 2).SetEase(closeAnimCurve).OnComplete(() =>
        {
            background.DOFade(0f, backgroundCloseTime).OnComplete(() => 
            {
                Color bgColor = background.color;
                bgColor.a = backgroundOpenAlpha;
                background.color = bgColor;

                Vector3 local = title.transform.localPosition;
                local.y = titleLocalY;
                title.transform.localPosition = local;

                local = stars.transform.localPosition;
                local.y = starsLocalY;
                stars.transform.localPosition = local;

                local = buttons.transform.localPosition;
                local.y = buttonsLocalY;
                buttons.transform.localPosition = local;

                onComplete?.Invoke(); 
            });
        });
    }

    string GetStarJumpsText(int jumps)
    {
        int numStart = starJumpsTemplate.IndexOf('{');
        int numEnd = starJumpsTemplate.IndexOf('}');
        return starJumpsTemplate[..numStart] + jumps.ToString() + starJumpsTemplate.Substring(numEnd + 1, starJumpsTemplate.Length - numEnd - 1);
    }

    void UpdateStars(int stars)
    {
        star3.SetStarActive(stars == 3);
        star2.SetStarActive(stars >= 2);
        star1.SetStarActive(stars >= 1);
    }
}
