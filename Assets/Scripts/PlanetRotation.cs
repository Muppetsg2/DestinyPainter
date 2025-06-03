using UnityEngine;

[RequireComponent (typeof(Planet))]
public class PlanetRotation : MonoBehaviour
{
    public enum RotationMode
    {
        Clockwise = -1,
        CounterClockwise = 1,
        None = 0
    }

    public float rotationSpeed = 20f;
    public RotationMode rotationMode = RotationMode.CounterClockwise;
    public Transform arrow1;
    public Transform arrow2;
    public bool hideArrows = false;

    void Start()
    {
        if (GetComponent<Planet>().isEnd) return;

        if (rotationMode != RotationMode.None && !hideArrows)
        {
            arrow1.gameObject.SetActive(true);
            arrow2.gameObject.SetActive(true);

            if (rotationMode == RotationMode.CounterClockwise)
            {
                arrow1.localScale = new Vector3(-arrow1.localScale.x, arrow1.localScale.y, arrow1.localScale.z);
                arrow2.localScale = new Vector3(-arrow2.localScale.x, arrow2.localScale.y, arrow2.localScale.z);
            }
        }
    }

    void Update()
    {
        transform.Rotate(0f, 0f, (int)rotationMode * rotationSpeed * Time.deltaTime);
    }
}