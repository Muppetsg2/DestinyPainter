using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPlanetSnap : MonoBehaviour
{
    public Camera mainCamera;
    public BoxCollider2D boundries;
    public PlayerController player;
    public float planetFocusHeightOffset;

    public bool end = false;

    private void Start()
    {
        mainCamera.orthographicSize = boundries.bounds.size.x * 0.5f / mainCamera.aspect;

        Vector3 minBoundries = boundries.bounds.min;
        Vector3 maxBoundries = boundries.bounds.max;

        Vector3 camPos = new(mainCamera.transform.position.x, player.currentPlanet.transform.position.y - planetFocusHeightOffset, mainCamera.transform.position.z);
        if (camPos.y + mainCamera.orthographicSize > maxBoundries.y)
        {
            camPos.y -= camPos.y + mainCamera.orthographicSize - maxBoundries.y;
        }
        if (camPos.y - mainCamera.orthographicSize < minBoundries.y)
        {
            camPos.y += minBoundries.y - (camPos.y - mainCamera.orthographicSize);
        }
        mainCamera.transform.position = camPos;
    }

    void Update()
    {
        if (!end)
        {
            Vector3 minBoundries = boundries.bounds.min;
            Vector3 maxBoundries = boundries.bounds.max;

            Vector3 camPos = new(mainCamera.transform.position.x, player.currentPlanet.transform.position.y - planetFocusHeightOffset, mainCamera.transform.position.z);
            if (camPos.y + mainCamera.orthographicSize > maxBoundries.y)
            {
                camPos.y -= camPos.y + mainCamera.orthographicSize - maxBoundries.y;
            }
            if (camPos.y - mainCamera.orthographicSize < minBoundries.y)
            {
                camPos.y += minBoundries.y - (camPos.y - mainCamera.orthographicSize);
            }
            mainCamera.transform.position += (camPos - mainCamera.transform.position) * Time.deltaTime;

            if (Vector3.Distance(mainCamera.transform.position, camPos) < 0.01f)
            {
                mainCamera.transform.position = camPos;
            }
        }
        else
        {
            Vector3 camPos = new(mainCamera.transform.position.x, boundries.bounds.center.y, mainCamera.transform.position.z);
            mainCamera.transform.position += (camPos - mainCamera.transform.position) * Time.deltaTime;

            if (Vector3.Distance(mainCamera.transform.position, camPos) < 0.01f)
            {
                mainCamera.transform.position = camPos;
            }

            float endCameraSize = boundries.bounds.size.y * 0.5f;

            mainCamera.orthographicSize += (endCameraSize - mainCamera.orthographicSize) * Time.deltaTime;
            if (endCameraSize - mainCamera.orthographicSize < 0.01f)
            {
                mainCamera.orthographicSize = endCameraSize;
            }
        }
    }
}
