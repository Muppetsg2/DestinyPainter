using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraColor : MonoBehaviour
{
    private void Start()
    {
        Camera cam = GetComponent<Camera>();
        cam.backgroundColor = ColorsManager.Instance.GetBackgroundColor();
    }
}
