using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    public float rotationSpeed = 20f;
    public bool rotateClockwise = true;

    void Update()
    {
        float direction = rotateClockwise ? -1 : 1;
        transform.Rotate(0f, 0f, direction * rotationSpeed * Time.deltaTime);
    }
}