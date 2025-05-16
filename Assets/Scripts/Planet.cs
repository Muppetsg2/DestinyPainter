using UnityEngine;
using static ColorsManager;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Planet : MonoBehaviour
{
    public bool isEnd;
    public bool isDeadly;
    public float playerRadius;
    public MulticolorPlanet multicolorPlanet;
    public ColorChangingPlanet colorChangingPlanet;
    public ColorType color;

    public bool changeMaterial = true;

    void Start()
    {
        gameObject.tag = "Planet";
        multicolorPlanet = GetComponent<MulticolorPlanet>();
        colorChangingPlanet = GetComponent<ColorChangingPlanet>();
        if (multicolorPlanet == null && colorChangingPlanet == null && changeMaterial)
        {
            GetComponent<SpriteRenderer>().material = ColorsManager.Instance.GetMaterial(color);
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

        if (secondary)
        {
            return ColorsManager.Instance.GetSecondaryColor(color);
        }
        else
        {
            return ColorsManager.Instance.GetPrimaryColor(color);
        }
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