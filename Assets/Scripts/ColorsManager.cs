using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ColorsManager : MonoBehaviour
{
    private static ColorsManager instance;
    public static ColorsManager Instance => instance;

    [Header("Color Data")]
    [SerializeField] private ColorPalette palette;

    [Header("Material")]
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material pickupMaterial;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one ColorsManager found in scene");
            DestroyImmediate(this);
            return;
        }

        instance = this;
        palette.Initialize();
    }

    public Color GetColor(ColorType type, ColorCategory category)
    {
        return palette.GetColor(type, category);
    }

    public Color GetBackgroundColor()
    {
        return palette.backgroundColor;
    }

    public Color GetStarColor(ColorCategory category, bool active)
    {
        return category switch
        {
            ColorCategory.Primary => active ? palette.primaryStarActiveColor : palette.primaryStarInactiveColor,
            ColorCategory.Secondary => active ? palette.secondaryStarActiveColor : palette.secondaryStarInactiveColor,
            ColorCategory.PrimaryHDR => active ? palette.primaryStarActiveHDRColor : palette.primaryStarInactiveHDRColor,
            ColorCategory.SecondaryHDR => active ? palette.secondaryStarActiveHDRColor : palette.secondaryStarInactiveHDRColor,
            _ => Color.black
        };
    }

    public Material GetMaterial(ColorType type)
    {
        Material mat = new Material(baseMaterial);
        mat.SetColor("_Outer", GetColor(type, ColorCategory.Primary));
        mat.SetColor("_Center", GetColor(type, ColorCategory.Secondary));
        return mat;
    }

    public Material GetPickupMaterial(ColorType type)
    {
        Material mat = new Material(pickupMaterial);
        mat.SetColor("_Center", GetColor(type, ColorCategory.PrimaryHDR));
        mat.SetColor("_Outer", GetColor(type, ColorCategory.SecondaryHDR));
        return mat;
    }
}
