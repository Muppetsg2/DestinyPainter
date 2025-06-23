using UnityEngine;

public class SplashWall : MonoBehaviour
{
    public Camera cam;
    public BoxCollider wallCollider;

    void Awake()
    {
        Vector3 size = wallCollider.size;
        size.x = -2f * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * cam.aspect * (cam.transform.position.z - wallCollider.transform.position.z);
        size.y = size.x / cam.aspect;
        wallCollider.size = size;
    }
}
