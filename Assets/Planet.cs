using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Planet : MonoBehaviour
{
    public bool isDeadly;

    void Start()
    {
        gameObject.tag = "Planet";
    }
}