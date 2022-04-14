using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraController : SingletonBehaviour<CameraController>
{
    [SerializeField] private Transform[] textureDisplayTransforms;
    [SerializeField] private Camera cameraRaw;
    [SerializeField] private Camera cameraHybrid;
    [SerializeField] private TMP_Text renderModeDisplayText;
    [SerializeField] private bool smooth;

    [SerializeField] public CameraSmartTarget SmartTarget;

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

    public static Vector2 ScreenToWorldPoint(Vector2 vector2)
    {
        return Instance.ActiveCamera().ScreenToWorldPoint(vector2);
    }

    internal static Vector3 WorldToScreenPoint(Vector3 worldPos)
    {
        return Instance.ActiveCamera().WorldToScreenPoint(worldPos);
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

        //transposer.m_TrackedObjectOffset = 
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

    private float RoundTo8PixelPerUnit(float raw)
    {

        float rounded = Mathf.Round(raw * 8f) / 8f;
        return rounded;
    }

    public enum RenderModes
    {
        RAW,
        HYBRID,
    }
}
