using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StarObject : MonoBehaviour
{
    [SerializeField] private bool active = false;
    [SerializeField] private Image image;
    [SerializeField] private Shadow shadow;
    [SerializeField] private TextMeshProUGUI textHolder;
    [SerializeField][TextArea] private string text = "";

    void Start()
    {
        SetStarActive(active);
        SetStarText(text);
    }

    private void SetStarColor()
    {
        image.color = ColorsManager.Instance.GetStarColor(ColorCategory.Primary, active);
        shadow.effectColor = ColorsManager.Instance.GetStarColor(ColorCategory.Secondary, active);
    }

    public void SetStarActive(bool value)
    {
        if (value == active) return;

        active = value;
        SetStarColor();
    }

    public void SetStarText(string value)
    {
        text = value;
        textHolder.text = text;
    }
}
