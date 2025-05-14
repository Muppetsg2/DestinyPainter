using System.Collections.Generic;
using UnityEngine;
using static ColorsManager;

public class MulticolorPlanet : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer spriteRenderer;
    protected MaterialPropertyBlock mPB;
    public List<float> baseSegments;
    public List<float> segments;
    public List<ColorType> segmentColors;

    private void Awake()
    {
        mPB = new MaterialPropertyBlock();
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitializeMaterial();
    }

    private void FixedUpdate()
    {
        UpdateSegments();
    }

    public void InitializeMaterial()
    {
        spriteRenderer.GetPropertyBlock(mPB);

        if (baseSegments.Count != segmentColors.Count + 1)
        {
            Debug.LogError("Segments count error.");
            return;
        }

        mPB.SetInt("_SegmentCount", segmentColors.Count);
        mPB.SetFloatArray("_Angles", baseSegments);

        var colorVecs = new List<Vector4>();
        var secondaryColorVecs = new List<Vector4>();
        foreach (ColorType c in segmentColors)
        {
            colorVecs.Add(ColorsManager.Instance.GetPrimaryColor(c));
            secondaryColorVecs.Add(ColorsManager.Instance.GetSecondaryColor(c));
        }

        mPB.SetVectorArray("_Colors", colorVecs);
        mPB.SetVectorArray("_SecondaryColors", secondaryColorVecs);

        spriteRenderer.SetPropertyBlock(mPB);
    }

    private void UpdateSegments()
    {
        for (int i = 0; i < segments.Count; ++i)
        {
            segments[i] = baseSegments[i] + transform.rotation.eulerAngles.z;
            if (segments[i] > 360)
            {
                segments[i] -= 360;
            }
        }
    }

    public bool CheckIsCorrectColor(float playerRotation, ColorType color)
    {
        while (playerRotation > 360)
        {
            playerRotation -= 360;
        }

        for (int i = 0; i < segments.Count - 1; ++i)
        {
            if (CheckIsInSegment(playerRotation, segments[i], segments[i+1]))
            {
                if (color == segmentColors[i])
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        return false;
    }

    private bool CheckIsInSegment(float value, float a, float b)
    {
        if (b > a)
        {
            return value >= a && value < b;
        }
        else if (value >= a)
        {
            return value >= a && value < b + 360;
        }
        else
        {
            return value + 360 >= a && value + 360 < b + 360;
        }
    }

    public Color GetColor(float playerAngle, bool secondary)
    {
        while (playerAngle > 360)
        {
            playerAngle -= 360;
        }

        for (int i = 0; i < segments.Count - 1; ++i)
        {
            if (CheckIsInSegment(playerAngle, segments[i], segments[i + 1]))
            {
                return GetColor(segmentColors[i], secondary);
            }
        }

        return Color.white;
    }

    private Color GetColor(ColorType c, bool secondary)
    {
        if (secondary)
        {
            return ColorsManager.Instance.GetSecondaryColor(c);
        }
        else
        {
            return ColorsManager.Instance.GetPrimaryColor(c);
        }
    }
}
