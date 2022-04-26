using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraController : SingletonBehaviour<CameraController>
{
    [SerializeField] private Transform[] textureDisplayTransforms;
    [SerializeField] private Camera cameraRaw;
    [SerializeField] private Camera cameraHybrid;
    [SerializeField] private Camera[] additionalCameras;
    [SerializeField] private PixelPerfectCamera pixelPerfectCamera;
    [SerializeField] private TMP_Text renderModeDisplayText;
    [SerializeField] private bool smooth;

    [SerializeField] public CameraSmartTarget SmartTarget;

    public const float DEFAULT_CAMERA_SIZE = 7.875f;
    [SerializeField] float orthographicSize = DEFAULT_CAMERA_SIZE;

    CinemachineVirtualCamera virtualCamera;
    CinemachineFramingTransposer transposer;

    List<RenderModes> allRenderModes = new List<RenderModes>();
    int renderModeIndex = 1;

    private void OnEnable()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        foreach (RenderModes mode in Enum.GetValues(typeof(RenderModes)))
            allRenderModes.Add(mode);

        UpdateRenderMode(allRenderModes[renderModeIndex]);

#if UNITY_EDITOR
        SceneVisibilityManager.instance.Hide(cameraHybrid.gameObject, true);
#endif
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
    }

    public Camera ActiveCamera()
    {
        RenderModes currentMode = allRenderModes[renderModeIndex];
        if (currentMode == RenderModes.RAW)
            return cameraRaw;

        return cameraHybrid;
    }

    private void UpdateRenderMode(RenderModes renderMode)
    {
        renderModeDisplayText.text = "RenderMode: " + renderMode;

        switch (renderMode)
        {
            case RenderModes.RAW:
                cameraRaw.gameObject.SetActive(true);
                cameraHybrid.gameObject.SetActive(false);
                break;

            case RenderModes.HYBRID:
                cameraRaw.gameObject.SetActive(false);
                cameraHybrid.gameObject.SetActive(true);
                break;
        }
    }

    private void LateUpdate()
    {
        SmoothCamera();
    }

    private void SmoothCamera()
    {
        Camera active = ActiveCamera();
        Vector2 camPosRaw = active.transform.position;
        Vector2 camPosRounded = new Vector2(RoundTo8PixelPerUnit(camPosRaw.x), RoundTo8PixelPerUnit(camPosRaw.y));

        foreach (Transform transform in textureDisplayTransforms)
        {
            transform.position = new Vector3(smooth ? camPosRounded.x : 0, smooth ? camPosRounded.y : 0, transform.position.z);
        }
    }

    public void SetCameraSize(float newSize)
    {
        orthographicSize = newSize;
        UpdateCameraSize();
    }

    [ContextMenu("UpdateCameraSize")]
    public void UpdateCameraSize()
    {
        virtualCamera.m_Lens.OrthographicSize = orthographicSize;
        cameraHybrid.orthographicSize = orthographicSize;
        foreach (Camera camera in additionalCameras) camera.orthographicSize = orthographicSize;
        pixelPerfectCamera.assetsPPU = Mathf.RoundToInt(63f / orthographicSize);
    }

    private float RoundTo8PixelPerUnit(float raw)
    {

        float rounded = Mathf.Round(raw * pixelPerfectCamera.assetsPPU) / pixelPerfectCamera.assetsPPU;
        return rounded;
    }

    public enum RenderModes
    {
        RAW,
        HYBRID,
    }
}
