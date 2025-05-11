using UnityEngine;
using static ColorsManager;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitializeMaterial();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        UpdateTransition();
    }

    public void InitializeMaterial()
    {
        spriteRenderer.GetPropertyBlock(mPB);

        mPB.SetVector("_OuterColor", ColorsManager.Instance.GetPrimaryColor(outerColor));

        mPB.SetVector("_InnerColor", ColorsManager.Instance.GetPrimaryColor(innerColor));

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

            mPB.SetVector("_OuterColor", ColorsManager.Instance.GetPrimaryColor(outerColor));

            mPB.SetVector("_InnerColor", ColorsManager.Instance.GetPrimaryColor(innerColor));
        }

        mPB.SetFloat("_BlendPoint", transition);

        spriteRenderer.SetPropertyBlock(mPB);
    }

    public bool CheckIsCorrectColor(ColorType playerColor)
    {
        if (playerColor == outerColor) return true;

        return false;
    }
}
