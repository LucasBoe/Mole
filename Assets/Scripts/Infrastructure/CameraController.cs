using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : SingletonBehaviour<CameraController>
{
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private Transform[] textureDisplayTransforms;
    [SerializeField] private Camera cameraPixel;
    [SerializeField] private Camera cameraRaw;
    [SerializeField] private bool smooth;

    CinemachineVirtualCamera virtualCamera;
    CinemachineFramingTransposer transposer;

    List<RenderModes> allRenderModes = new List<RenderModes>();
    int renderModeIndex = 1;

    private void OnEnable()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        PlayerController.OnPlayerSpawned += ConnectPlayerTransformToVirtualCamera;

        foreach (RenderModes mode in Enum.GetValues(typeof(RenderModes)))
            allRenderModes.Add(mode);
    }



    private void ConnectPlayerTransformToVirtualCamera(Transform player)
    {
        virtualCamera.Follow = player;
    }

    public static Vector2 ScreenToWorldPoint(Vector2 vector2)
    {
        Vector2 remap = new Vector2((vector2.x / Screen.width) * Instance.renderTexture.width, (vector2.y / Screen.height) * Instance.renderTexture.height);
        return Instance.cameraPixel.ScreenToWorldPoint(remap);
    }

    internal static Vector3 WorldToScreenPoint(Vector3 worldPos)
    {
        Vector2 raw = Instance.cameraPixel.WorldToScreenPoint(worldPos);
        return new Vector2((raw.x / Instance.renderTexture.width) * Screen.width, (raw.y / Instance.renderTexture.height) * Screen.height);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F3))
        {
            renderModeIndex--;
            if (renderModeIndex < 0)
                renderModeIndex = allRenderModes.Count - 1;

            UpdateRenderMode(allRenderModes[renderModeIndex]);
        }

        transposer.m_TrackedObjectOffset = PlayerInputHandler.PlayerInput.VirtualCursorToScreenCenter * new Vector2(4, 3);
    }

    private void UpdateRenderMode(RenderModes renderMode)
    {
        switch (renderMode)
        {
            case RenderModes.RAW:
                cameraRaw.gameObject.SetActive(true);
                cameraPixel.gameObject.SetActive(false);
                break;

            case RenderModes.PIXELOLD:
                cameraRaw.gameObject.SetActive(false);
                cameraPixel.gameObject.SetActive(true);
                break;
        }
    }

    private void LateUpdate()
    {
        SmoothCamera();
    }

    private void SmoothCamera()
    {
        Vector2 camPosRaw = cameraPixel.transform.position;
        Vector2 camPosRounded = new Vector2(RoundTo8PixelPerUnit(camPosRaw.x), RoundTo8PixelPerUnit(camPosRaw.y));
        Vector3 rest = (Vector3)(camPosRounded - camPosRaw);

        foreach (Transform transform in textureDisplayTransforms)
        {
            float z = transform.localPosition.z;
            transform.localPosition = new Vector3(smooth ? rest.x : 0, smooth ? rest.y : 0, z);
        }
    }

    private float RoundTo8PixelPerUnit(float raw)
    {
        return Mathf.Round(raw * 8f) / 8f;
    }

    public enum RenderModes
    {
        RAW,
        PIXELOLD,
        PIXELNEW,
    }
}
