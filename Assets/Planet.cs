using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Planet : MonoBehaviour
{
    public bool isEnd;
    public bool isDeadly;
    public float playerRadius;
    public bool isMulticolored;

    void Start()
    {
        gameObject.tag = "Planet";
    }
}