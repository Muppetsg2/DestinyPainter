using UnityEngine;

public class ColorsManager : MonoBehaviour
{
    private static ColorsManager instance;
    public static ColorsManager Instance => instance;

    [Header("Color Data")]
    [SerializeField] private ColorPalette palette;

    [Header("Material")]
    [SerializeField] private Material baseMaterial;

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

    public Color GetPrimaryColor(ColorType type)
    {
        return palette.GetPrimaryColor(type);
    }

    public Color GetSecondaryColor(ColorType type)
    {
        return palette.GetSecondaryColor(type);
    }

    public Material GetMaterial(ColorType type)
    {
        Material mat = new Material(baseMaterial);
        mat.SetColor("_Outer", GetPrimaryColor(type));
        mat.SetColor("_Center", GetSecondaryColor(type));
        return mat;
    }
}
