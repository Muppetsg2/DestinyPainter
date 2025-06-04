using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ColorChangePickup : MonoBehaviour
{
    public ColorType color;
    public float animationEndValue = 0.7f;
    public float animationDuration = 0.7f;
    [SerializeField] private bool grayOut = false;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.material = ColorsManager.Instance.GetPickupMaterial(color, grayOut);
        StartAnimation();
    }

    public void Pickup()
    {
        Destroy(GetComponent<SpriteRenderer>().material);
        GetComponent<SpriteRenderer>().material = ColorsManager.Instance.GetTakenPickupMaterial();
        StopAnimation();
    }

    public void FinalizePickup()
    {
        gameObject.SetActive(false);
    }

    public void Return()
    {
        Destroy(GetComponent<SpriteRenderer>().material);
        GetComponent<SpriteRenderer>().material = ColorsManager.Instance.GetPickupMaterial(color);
        gameObject.SetActive(true);
        StartAnimation();
    }

    public void StartAnimation()
    {
        transform.DOScale(animationEndValue, animationDuration).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopAnimation()
    {
        DOTween.Kill(transform);
    }

    public void SetGrayOut(bool value)
    {
        grayOut = value;
        sr.material = ColorsManager.Instance.GetPickupMaterial(color, grayOut);
    }
}