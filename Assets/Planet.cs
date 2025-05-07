using UnityEngine;
using static ColorsManager;

[RequireComponent(typeof(Collider2D))]
public class Planet : MonoBehaviour
{
    public bool isEnd;
    public bool isDeadly;
    public float playerRadius;
    public MulticolorPlanet multicolorPlanet;
    public ColorChangingPlanet colorChangingPlanet;
    public ColorType color;

    void Start()
    {
        gameObject.tag = "Planet";
        multicolorPlanet = GetComponent<MulticolorPlanet>();
        colorChangingPlanet = GetComponent<ColorChangingPlanet>();
    }

    public bool CheckIsCorrectColor(float playerRotation, ColorType playerColor)
    {
        if (colorChangingPlanet != null)
        {
            return colorChangingPlanet.CheckIsCorrectColor(playerColor);
        }
        else if (multicolorPlanet != null)
        {
            return multicolorPlanet.CheckIsCorrectColor(playerRotation, playerColor);
        }
        else
        {
            if (playerColor == color) return true;
        }

        return false;
    }
}