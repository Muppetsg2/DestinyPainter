using DG.Tweening;
using UnityEngine;

public class ColorChangePickup : MonoBehaviour
{
    public ColorType color;
    public float animationEndValue = 0.7f;
    public float animationDuration = 0.7f;

    void Start()
    {
        GetComponent<SpriteRenderer>().material = ColorsManager.Instance.GetPickupMaterial(color);
        StartAnimation();
    }

    public void Pickup()
    {
        StopAnimation();
        Destroy(gameObject);
    }

    public void StartAnimation()
    {
        transform.DOScale(animationEndValue, animationDuration).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopAnimation()
    {
        DOTween.Kill(transform);
    }
}