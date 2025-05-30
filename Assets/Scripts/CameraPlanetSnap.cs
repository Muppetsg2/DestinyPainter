using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPlanetSnap : MonoBehaviour
{
    public Camera mainCamera;
    public BoxCollider2D boundries;
    public PlayerController player;
    public float planetFocusHeightOffset;

    public float endAnimTime = 2f;

    private bool end = false;

    private void SetCameraViewSize()
    {
        mainCamera.transform.position = GetCameraPos();

        if (mainCamera.orthographic)
        {
            mainCamera.orthographicSize = boundries.bounds.size.x * 0.5f / mainCamera.aspect;
        }
        else
        {
            float z = -boundries.bounds.size.x * 0.5f / (Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * mainCamera.aspect);
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, z);
        }
    }

    private Vector3 GetCameraPos()
    {
        Vector3 minBoundries = boundries.bounds.min;
        Vector3 maxBoundries = boundries.bounds.max;

        Vector3 camPos = new(mainCamera.transform.position.x, player.currentPlanet.transform.position.y - planetFocusHeightOffset, mainCamera.transform.position.z);

        float halfHeight;
        if (mainCamera.orthographic)
        {
            halfHeight = mainCamera.orthographicSize;
        }
        else
        {
            halfHeight = mainCamera.rect.height * 0.5f;
        }

        if (camPos.y + halfHeight > maxBoundries.y)
        {
            camPos.y -= camPos.y + halfHeight - maxBoundries.y;
        }
        if (camPos.y - halfHeight < minBoundries.y)
        {
            camPos.y += minBoundries.y - (camPos.y - halfHeight);
        }

        return camPos;
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        mainCamera = Camera.main;

        SetCameraViewSize();
    }

    void Update()
    {
        if (end) return;

        Vector3 camPos = GetCameraPos();
        mainCamera.transform.position += (camPos - mainCamera.transform.position) * Time.deltaTime;
        if (Vector3.Distance(mainCamera.transform.position, camPos) < 0.01f)
        {
            mainCamera.transform.position = camPos;
        }
    }

    public void PlayEndAnim(Action onComplete = null)
    {
        end = true;

        float z = mainCamera.transform.position.z;
        if (!mainCamera.orthographic)
        {
            z = -boundries.bounds.size.y * 0.5f / Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }
        Vector3 desiredPos = new(mainCamera.transform.position.x, boundries.bounds.center.y, z);
        mainCamera.transform.DOMove(desiredPos, endAnimTime).OnComplete(() => onComplete?.Invoke());

        if (mainCamera.orthographic)
        {
            float endCameraSize = boundries.bounds.size.y * 0.5f;
            mainCamera.DOOrthoSize(endCameraSize, endAnimTime);
        }
    }
}
