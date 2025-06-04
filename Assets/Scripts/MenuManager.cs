using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public RectTransform mainImage;
    public float moveMainImageTime = 2f;

    [Header("Menu")]
    public List<ButtonAnimation> menuButtons;
    public Transform continueBtnTransform;
    public GameObject menuLines;
    public Transform menuMainImageParent;
    public Vector3 mainImageMenuPos;

    [Header("Level Select")]
    public List<ButtonAnimation> levelSelectButtons;
    public GameObject levelSelectLines;
    public GameObject levelSelectBackButton;
    public Transform levelsMainImageParent;
    public Vector3 mainImageLevelSelectPos;
    public ScrollRect levelSelectScroll;
    public Image levelSelectViewport;
    public float levelSelectScrollAnimationTime = 1f;
    public float levelSelectScrollOffset = 20f;

    [Header("Credits")]
    public List<RectTransform> creditsPlanetsRectTransforms;
    public Transform creditsMainImageParent;
    public Vector3 mainImageCreditsPos;

    public enum CurrentView
    {
        None, Menu, LevelSelect, Settings, Credits
    }
    public CurrentView ViewType { get; private set; } = CurrentView.None;

    void Start()
    {
        // menu
        foreach (var button in menuButtons)
        {
            button.GetComponent<Button>().enabled = false;
            button.gameObject.SetActive(false);
        }
        menuLines.SetActive(false);
        mainImage.anchoredPosition = mainImageMenuPos;

        // levels
        foreach (var button in levelSelectButtons)
        {
            button.GetComponent<Button>().enabled = false;
            button.gameObject.SetActive(false);
        }
        levelSelectLines.SetActive(false);
        levelSelectBackButton.SetActive(false);
        levelSelectScroll.enabled = false;
        levelSelectViewport.raycastTarget = false;

        OpenMenu();
    }

    private void ShowAnimation(List<ButtonAnimation> buttons, GameObject lines, CurrentView endViewType, Transform mainImageParent, Vector3 mainImagePos, Action onComplete = null)
    {
        void AnimateButtons()
        {
            lines.SetActive(true);
            foreach (var button in buttons)
            {
                button.gameObject.SetActive(true);
                button.transform.position = mainImage.position;
                button.PlayShow(() =>
                {
                    foreach (var button in buttons)
                    {
                        if (button.AnimType != ButtonAnimation.AnimationType.Idle)
                        {
                            return;
                        }
                    }
                    ViewType = endViewType;
                    foreach (var button in buttons)
                    {
                        button.GetComponent<Button>().enabled = true;
                    }
                    mainImage.GetComponent<Button>().enabled = true;
                    onComplete?.Invoke();
                });
            }
        }

        if (Vector3.Distance(mainImagePos, mainImage.anchoredPosition) > 1f)
        {
            mainImage.DOAnchorPos3D(mainImagePos, moveMainImageTime).OnComplete(() =>
            {
                mainImage.SetParent(mainImageParent);
                AnimateButtons();
            });
        }
        else
        {
            AnimateButtons();
        }
    }

    private void HideAnimation(List<ButtonAnimation> buttons, GameObject lines, Action onComplete = null)
    {
        mainImage.GetComponent<Button>().enabled = false;
        foreach (var button in buttons)
        {
            button.GetComponent<Button>().enabled = false;
            button.StopIdle();
            button.PlayHide(() =>
            {
                button.gameObject.SetActive(false);
                foreach (var button in buttons)
                {
                    if (button.gameObject.activeSelf)
                    {
                        return;
                    }
                }
                lines.SetActive(false);
                ViewType = CurrentView.None;
                onComplete?.Invoke();
            });
        }
    }

    private void ShowMenu(Action onComplete = null)
    {
        if (ViewType != CurrentView.None) return;
        ShowAnimation(menuButtons, menuLines, CurrentView.Menu, menuMainImageParent, mainImageMenuPos, onComplete);
    }

    private void HideMenu(Action onComplete = null)
    {
        if (ViewType != CurrentView.Menu) return;
        HideAnimation(menuButtons, menuLines, onComplete);
    }

    private void ShowLevelSelect(Action onComplete = null)
    {
        if (ViewType != CurrentView.None) return;
        ShowAnimation(levelSelectButtons, levelSelectLines, CurrentView.LevelSelect, levelsMainImageParent, mainImageLevelSelectPos, () => 
        {
            levelSelectScroll.enabled = true;

            float buttonTransformY = levelSelectButtons[0].GetComponent<RectTransform>().anchoredPosition.y;
            foreach (var button in levelSelectButtons)
            {
                if (button.GetComponent<LevelButton>().IsUnlocked())
                {
                    buttonTransformY = button.GetComponent<RectTransform>().anchoredPosition.y;
                }
                else
                {
                    break;
                }
            }

            float percentage = Mathf.Clamp01(-(buttonTransformY + levelSelectScrollOffset) / levelSelectScroll.content.sizeDelta.y);
            float time = levelSelectScrollAnimationTime * Mathf.Abs(percentage - levelSelectScroll.verticalNormalizedPosition);
            levelSelectScroll.DOVerticalNormalizedPos(percentage, time).OnComplete(() => 
            {
                levelSelectViewport.raycastTarget = true;
                levelSelectBackButton.SetActive(true);
                onComplete?.Invoke();
            });
        });
    }

    private void HideLevelSelect(Action onComplete = null)
    {
        if (ViewType != CurrentView.LevelSelect) return;
        levelSelectBackButton.SetActive(false);
        levelSelectViewport.raycastTarget = false;

        float time = levelSelectScrollAnimationTime * (1f - levelSelectScroll.verticalNormalizedPosition);
        levelSelectScroll.DOVerticalNormalizedPos(1f, time).OnComplete(() =>
        {
            levelSelectScroll.enabled = false;
            HideAnimation(levelSelectButtons, levelSelectLines, onComplete);
        });
    }

    private void ShowCredits(Action onComplete = null)
    {

    }

    private void HideCredits(Action onComplete = null)
    {

    }

    private void HideCurrent(Action onComplete = null)
    {
        switch (ViewType)
        {
            case CurrentView.LevelSelect:
                HideLevelSelect(onComplete);
                break;
            case CurrentView.Menu:
                HideMenu(onComplete);
                break;
            case CurrentView.Credits:
                HideCredits(onComplete);
                break;
            default:
                onComplete?.Invoke();
                break;
        }
    }

    public void OpenMenu()
    {
        HideCurrent(() => ShowMenu());
    }

    public void OpenLevelSelect()
    {
        HideCurrent(() => ShowLevelSelect());
    }

    public void OpenCredits()
    {
        HideCurrent(() => ShowCredits());
    }

    public void Continue()
    {
        LevelData lastLevelData = levelSelectButtons[0].GetComponent<LevelButton>().data;
        foreach (var button in levelSelectButtons)
        {
            var levelButton = button.GetComponent<LevelButton>();
            if (levelButton.IsUnlocked())
            {
                lastLevelData = levelButton.data;
            }
            else
            {
                break;
            }
        }
        if (lastLevelData == null) return;
        continueBtnTransform.GetComponent<ButtonAnimation>().StopIdle();
        lastLevelData.LoadLevel();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OpenPaintBalls()
    {
        LevelLoader.Instance.LoadLevel("Paint Balls");
    }
}
