using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Colors;

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
        foreach (ColorType c in segmentColors)
        {
            colorVecs.Add(colorsDict[c]);
        }

        mPB.SetVectorArray("_Colors", colorVecs);

        spriteRenderer.SetPropertyBlock(mPB);
    }

    private void UpdateSegments()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i] = baseSegments[i] + transform.rotation.eulerAngles.z;
            if (segments[i] > 360)
            {
                segments[i] -= 360;
            }
        }
    }

    public bool CheckIsCorrectColor(float rotation, ColorType color)
    {
        float playerRotation = rotation + 90;

        if (playerRotation > 360)
        {
            playerRotation -= 360;
        }

        for (int i = 0; i < segments.Count - 1; i++)
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
}
