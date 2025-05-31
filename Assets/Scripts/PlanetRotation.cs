using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    public float rotationSpeed = 20f;
    [Tooltip("-1 is clockwise; 1 is counter clockwise; 0 static")][Range(-1, 1)] 
    public int rotationMode = -1;
    public Transform arrow1;
    public Transform arrow2;
    public bool hideArrows = false;

    void Start()
    {
        if (rotationMode != 0 && !hideArrows)
        {
            arrow1.gameObject.SetActive(true);
            arrow2.gameObject.SetActive(true);

            if (rotationMode == 1)
            {
                arrow1.localScale = new Vector3(arrow1.localScale.x, -1 * arrow1.localScale.y, arrow1.localScale.z);
                arrow1.localPosition = new Vector3(arrow1.localPosition.x, -1 * arrow1.localPosition.y, arrow1.localPosition.z);

                arrow2.localScale = new Vector3(arrow2.localScale.x, -1 * arrow2.localScale.y, arrow2.localScale.z);
                arrow2.localPosition = new Vector3(arrow2.localPosition.x, -1 * arrow2.localPosition.y, arrow2.localPosition.z);
            }
        }
    }

    void Update()
    {
        transform.Rotate(0f, 0f, rotationMode * rotationSpeed * Time.deltaTime);
    }
}