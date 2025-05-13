using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPlanetSnap : MonoBehaviour
{
    public Camera mainCamera;
    public BoxCollider2D boundries;
    public PlayerController player;
    public float planetFocusHeightOffset;

    public bool end = false;

    private Vector3 CalculateCameraPos()
    {
        Vector3 camPos;
        if (!end)
        {
            Vector3 minBoundries = boundries.bounds.min;
            Vector3 maxBoundries = boundries.bounds.max;

            camPos = new(mainCamera.transform.position.x, player.currentPlanet.transform.position.y - planetFocusHeightOffset, mainCamera.transform.position.z);
            if (camPos.y + mainCamera.orthographicSize > maxBoundries.y)
            {
                camPos.y -= camPos.y + mainCamera.orthographicSize - maxBoundries.y;
            }
            if (camPos.y - mainCamera.orthographicSize < minBoundries.y)
            {
                camPos.y += minBoundries.y - (camPos.y - mainCamera.orthographicSize);
            }
        }
        else
        {
            float z = mainCamera.transform.position.z;
            if (!mainCamera.orthographic)
            {
                z = -boundries.bounds.size.y * 0.5f / Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            }
            camPos = new(mainCamera.transform.position.x, boundries.bounds.center.y, z);
            mainCamera.transform.position += (camPos - mainCamera.transform.position) * Time.deltaTime;
        }

        return camPos;
    }

    private void Start()
    {
        if (mainCamera.orthographic)
        {
            mainCamera.orthographicSize = boundries.bounds.size.x * 0.5f / mainCamera.aspect;
        }

        mainCamera.transform.position = CalculateCameraPos();
        if (!mainCamera.orthographic)
        {
            float z = -boundries.bounds.size.x * 0.5f / (Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * mainCamera.aspect);
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, z);
        }
    }

    void Update()
    {
        Vector3 camPos = CalculateCameraPos();
        mainCamera.transform.position += (camPos - mainCamera.transform.position) * Time.deltaTime;
        if (Vector3.Distance(mainCamera.transform.position, camPos) < 0.01f)
        {
            mainCamera.transform.position = camPos;
        }

        if (end && mainCamera.orthographic)
        {
            float endCameraSize = boundries.bounds.size.y * 0.5f;

            mainCamera.orthographicSize += (endCameraSize - mainCamera.orthographicSize) * Time.deltaTime;
            if (endCameraSize - mainCamera.orthographicSize < 0.01f)
            {
                mainCamera.orthographicSize = endCameraSize;
            }
        }
    }
}
