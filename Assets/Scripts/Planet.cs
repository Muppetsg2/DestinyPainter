using SaintsField;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Planet : MonoBehaviour
{
    public bool isEnd;
    public bool isDeadly;
    [ShowIf("isDeadly"), SerializeField] private TextMeshProUGUI text;
    public MulticolorPlanet multicolorPlanet;
    public ColorChangingPlanet colorChangingPlanet;
    public ColorType color;

    public bool changeMaterial = true;

    private bool radiusIsDirty = false;

    [ReadOnly] public float playerRadius;

    private string baseText = "Loremipsumdolorzsf";

    public void OnValidate()
    {
        ForceUpdateRadius();
    }

    void Start()
    {
        gameObject.tag = "Planet";
        multicolorPlanet = GetComponent<MulticolorPlanet>();
        colorChangingPlanet = GetComponent<ColorChangingPlanet>();
        if (multicolorPlanet == null && colorChangingPlanet == null && changeMaterial)
        {
            GetComponent<SpriteRenderer>().material = ColorsManager.Instance.GetMaterial(color);
        }

        if (isDeadly && text != null)
        {
            text.SetText(ShuffleText(baseText));
        }
    }

    private void Update()
    {
        if (radiusIsDirty)
        {
            ForceUpdateRadius();
        }
    }

    public bool CheckIsCorrectColor(float playerAngle, ColorType playerColor)
    {
        if (colorChangingPlanet != null)
        {
            return colorChangingPlanet.CheckIsCorrectColor(playerColor);
        }
        else if (multicolorPlanet != null)
        {
            return multicolorPlanet.CheckIsCorrectColor(playerAngle, playerColor);
        }
        else
        {
            if (playerColor == color) return true;
        }

        return false;
    }

    public string ShuffleText(string textToShuffle)
    {
        if (string.IsNullOrEmpty(textToShuffle))
        {
            return textToShuffle;
        }

        char[] characters = textToShuffle.ToCharArray();
        int n = characters.Length;
        for (int i = n - 1; i > 0; --i)
        {
            int j = Random.Range(0, i + 1);
            char temp = characters[i];
            characters[i] = characters[j];
            characters[j] = temp;
        }
        return new string(characters);
    }

    public void SetRadiusDirty()
    {
        radiusIsDirty = true;
    }

    public void SetRadiusClean()
    {
        radiusIsDirty = false;
    }

    public void ForceUpdateRadius()
    {
        playerRadius = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y) * 0.5f + 0.11f;
    }

    public bool IsMulticolor()
    {
        return multicolorPlanet != null;
    }

    public bool IsColorChanging()
    {
        return colorChangingPlanet != null;
    }

    public Color GetColor(float playerAngle, bool secondary)
    {
        if (colorChangingPlanet != null)
        {
            return colorChangingPlanet.GetColor(playerAngle, secondary);
        }
        else if (multicolorPlanet != null)
        {
            return multicolorPlanet.GetColor(playerAngle, secondary);
        }

        return ColorsManager.Instance.GetColor(color, secondary ? ColorCategory.Secondary : ColorCategory.Primary);
    }

    public void ChangePlanetColor(ColorType newColor)
    {
        if (multicolorPlanet == null && colorChangingPlanet == null && changeMaterial)
        {
            GetComponent<SpriteRenderer>().material = ColorsManager.Instance.GetMaterial(newColor);
            color = newColor;
        }
    }
}