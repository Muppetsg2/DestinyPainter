using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPlanetSnap : MonoBehaviour
{
    public Camera mainCamera;
    public PlayerController player;
    public float planetFocusHeight;
    public float bottomBoundrie;
    public float topBoundrie;

    public bool end = false;
    public float endCameraPoint;
    public float endCameraSize;

    void Update()
    {
        if (!end)
        {
            Vector3 camPos = new(mainCamera.transform.position.x, player.currentPlanet.transform.position.y - planetFocusHeight, mainCamera.transform.position.z);
            if (camPos.y + mainCamera.orthographicSize > topBoundrie)
            {
                camPos.y -= camPos.y + mainCamera.orthographicSize - topBoundrie;
            }
            if (camPos.y - mainCamera.orthographicSize < bottomBoundrie)
            {
                camPos.y += bottomBoundrie - (camPos.y - mainCamera.orthographicSize);
            }
            mainCamera.transform.position += (camPos - mainCamera.transform.position) * Time.deltaTime;

            if (Vector3.Distance(mainCamera.transform.position, camPos) < 0.05f)
            {
                mainCamera.transform.position = camPos;
            }
        }
        else
        {
            Vector3 camPos = new(mainCamera.transform.position.x, endCameraPoint, mainCamera.transform.position.z);
            mainCamera.transform.position += (camPos - mainCamera.transform.position) * Time.deltaTime;

            if (Vector3.Distance(mainCamera.transform.position, camPos) < 0.05f)
            {
                mainCamera.transform.position = camPos;
            }

            mainCamera.orthographicSize += (endCameraSize - mainCamera.orthographicSize) * Time.deltaTime;
            if (endCameraSize - mainCamera.orthographicSize < 0.05f)
            {
                mainCamera.orthographicSize = endCameraSize;
            }
        }
    }
}
