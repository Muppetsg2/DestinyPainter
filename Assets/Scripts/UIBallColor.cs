using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIBallColor : MonoBehaviour
{
    public ColorType color;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.material = new Material(image.material);
    }

    void Start()
    {
        UpdateColor();
    }

    public void SetColor(ColorType color)
    {
        this.color = color;
        UpdateColor();
    }

    void UpdateColor()
    {
        image.material.SetColor("_Outer", ColorsManager.Instance.GetColor(color, ColorCategory.Primary));
        image.material.SetColor("_Center", ColorsManager.Instance.GetColor(color, ColorCategory.Secondary));
    }
}
