using UnityEngine;

public class ColorChangingPlanet : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer spriteRenderer;
    protected MaterialPropertyBlock mPB;
    public ColorType outerColor;
    public ColorType innerColor;

    public float transition = 0;
    public ColorType activeColor;
    public float borderWidth = 0.05f;

    public float transitionDuration;

    private void Awake()
    {
        mPB = new MaterialPropertyBlock();
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitializeMaterial();
    }

    private void Update()
    {
        if (LevelManager.Instance.isPaused) return;

        UpdateTransition();
    }

    public void InitializeMaterial()
    {
        spriteRenderer.GetPropertyBlock(mPB);

        mPB.SetVector("_OuterColor", ColorsManager.Instance.GetColor(outerColor, ColorCategory.Primary));

        mPB.SetVector("_InnerColor", ColorsManager.Instance.GetColor(innerColor, ColorCategory.Primary));

        mPB.SetFloat("_BlendPoint", transition);

        mPB.SetFloat("_UseInnerColorForBorder", borderWidth);

        mPB.SetFloat("_BorderWidth", borderWidth);

        spriteRenderer.SetPropertyBlock(mPB);
    }

    private void UpdateTransition()
    {
        spriteRenderer.GetPropertyBlock(mPB);

        transition += Time.deltaTime / transitionDuration;
        
        if (transition > 1)
        {
            (innerColor, outerColor) = (outerColor, innerColor);
            
            transition %= 1;

            mPB.SetVector("_OuterColor", ColorsManager.Instance.GetColor(outerColor, ColorCategory.Primary));

            mPB.SetVector("_InnerColor", ColorsManager.Instance.GetColor(innerColor, ColorCategory.Primary));
        }

        mPB.SetFloat("_BlendPoint", transition);

        spriteRenderer.SetPropertyBlock(mPB);
    }

    public bool CheckIsCorrectColor(ColorType playerColor)
    {
        if (playerColor == outerColor) return true;

        return false;
    }

    public Color GetColor(float playerAngle, bool secondary)
    {
        if (secondary)
        {
            return ColorsManager.Instance.GetColor(outerColor, ColorCategory.Secondary);
        }
        else
        {
            return ColorsManager.Instance.GetColor(outerColor, ColorCategory.Primary);
        }
    }
}
