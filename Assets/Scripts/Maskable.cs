using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class Maskable : UIBehaviour, ICanvasRaycastFilter, IMaterialModifier
{
    [NonSerialized]
    private RectTransform m_RectTransform;
    public RectTransform rectTransform
    {
        get { return m_RectTransform ?? (m_RectTransform = GetComponent<RectTransform>()); }
    }

    [SerializeField]
    private bool m_ShowInsideMask = false;

    public bool showInsideMask
    {
        get { return m_ShowInsideMask; }
        set
        {
            if (m_ShowInsideMask == value)
                return;

            m_ShowInsideMask = value;
            if (graphic != null)
                graphic.SetMaterialDirty();
        }
    }

    [NonSerialized]
    private Graphic m_Graphic;

    /// <summary>
    /// The graphic associated with the Mask.
    /// </summary>
    public Graphic graphic
    {
        get { return m_Graphic ?? (m_Graphic = GetComponent<Graphic>()); }
    }

    [NonSerialized]
    private Material m_MaskableMaterial;
    [NonSerialized]
    private Material m_UnmaskMaterial;

    private Mask parent;

    protected Maskable()
    { }

    public virtual bool MaskableEnabled() { return IsActive() && graphic != null && parent != null; }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (graphic != null)
        {
            graphic.canvasRenderer.hasPopInstruction = true;
            graphic.SetMaterialDirty();
        }

        MaskUtilities.NotifyStencilStateChanged(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (graphic != null)
        {
            graphic.SetMaterialDirty();
            graphic.canvasRenderer.hasPopInstruction = false;
            graphic.canvasRenderer.popMaterialCount = 0;
        }

        StencilMaterial.Remove(m_MaskableMaterial);
        m_MaskableMaterial = null;
        StencilMaterial.Remove(m_UnmaskMaterial);
        m_UnmaskMaterial = null;

        MaskUtilities.NotifyStencilStateChanged(this);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        if (!IsActive())
            return;

        if (graphic != null)
        {
            graphic.SetMaterialDirty();
        }

        MaskUtilities.NotifyStencilStateChanged(this);
    }

#endif

    public void EnableMask(Mask parent)
    {
        this.parent = parent;

        if (graphic != null)
        {
            graphic.SetMaterialDirty();
        }

        MaskUtilities.NotifyStencilStateChanged(this);
    }

    public void DisableMask()
    {
        this.parent = null;

        if (graphic != null)
        {
            graphic.SetMaterialDirty();
        }

        MaskUtilities.NotifyStencilStateChanged(this);
    }

    private bool setPopMaterial = false;
    public void SetPopMaterial(bool value)
    {
        setPopMaterial = value;
    }

    public virtual bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (!isActiveAndEnabled)
            return true;

        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, sp, eventCamera);
    }

    /// Stencil calculation time!
    public virtual Material GetModifiedMaterial(Material baseMaterial)
    {
        if (!MaskableEnabled())
            return baseMaterial;

        var stencilDepth = parent.stencilDepth;

        int desiredStencilBit = 1 << stencilDepth;

        // if we are at the first level...
        // we want to destroy what is there
        if (desiredStencilBit == 1)
        {
            var maskableMaterial = StencilMaterial.Add(baseMaterial, desiredStencilBit + (m_ShowInsideMask ? 0 : 1), StencilOp.Keep, CompareFunction.Equal, ColorWriteMask.All, 1, 0);
            StencilMaterial.Remove(m_MaskableMaterial);
            m_MaskableMaterial = maskableMaterial;

            var unmaskMaterial = StencilMaterial.Add(baseMaterial, 1, StencilOp.Zero, CompareFunction.Always, 0);
            StencilMaterial.Remove(m_UnmaskMaterial);
            m_UnmaskMaterial = unmaskMaterial;
            if (setPopMaterial)
            {
                graphic.canvasRenderer.popMaterialCount = 1;
                graphic.canvasRenderer.SetPopMaterial(m_UnmaskMaterial, 0);
            }

            return m_MaskableMaterial;
        }

        var maskableMaterial2 = StencilMaterial.Add(baseMaterial, desiredStencilBit - 1 + (m_ShowInsideMask ? 0 : 1), StencilOp.Replace, CompareFunction.Equal, ColorWriteMask.All, desiredStencilBit - 1, desiredStencilBit | (desiredStencilBit - 1));
        StencilMaterial.Remove(m_MaskableMaterial);
        m_MaskableMaterial = maskableMaterial2;

        graphic.canvasRenderer.hasPopInstruction = setPopMaterial;
        var unmaskMaterial2 = StencilMaterial.Add(baseMaterial, desiredStencilBit - 1, StencilOp.Replace, CompareFunction.Equal, 0, desiredStencilBit - 1, desiredStencilBit | (desiredStencilBit - 1));
        StencilMaterial.Remove(m_UnmaskMaterial);
        m_UnmaskMaterial = unmaskMaterial2;
        if (setPopMaterial)
        {
            graphic.canvasRenderer.popMaterialCount = 1;
            graphic.canvasRenderer.SetPopMaterial(m_UnmaskMaterial, 0);
        }

        return m_MaskableMaterial;
    }
}
