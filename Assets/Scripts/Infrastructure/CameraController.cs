using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : SingletonBehaviour<CameraController>
{
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private Transform[] textureDisplayTransforms;
    [SerializeField] private Camera cameraRaw;
    [SerializeField] private Camera cameraPixel;
    [SerializeField] private Camera cameraPixel2;
    [SerializeField] private TMP_Text renderModeDisplayText;
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

        UpdateRenderMode(allRenderModes[renderModeIndex]);
    }



    private void ConnectPlayerTransformToVirtualCamera(Transform player)
    {
        virtualCamera.Follow = player;
    }

    public static Vector2 ScreenToWorldPoint(Vector2 vector2)
    {
        Vector2 multiplier = Instance.GetMultiplierByRenderMode();
        Vector2 remap = new Vector2((vector2.x / Screen.width) * multiplier.x, (vector2.y / Screen.height) * multiplier.y);
        return Instance.ActiveCamera().ScreenToWorldPoint(remap);
    }

    internal static Vector3 WorldToScreenPoint(Vector3 worldPos)
    {
        Vector2 multiplier = Instance.GetMultiplierByRenderMode();
        Vector2 raw = Instance.ActiveCamera().WorldToScreenPoint(worldPos);
        return new Vector2((raw.x / multiplier.x) * Screen.width, (raw.y / multiplier.y) * Screen.height);
    }


    private Vector2 GetMultiplierByRenderMode()
    {
        RenderModes currentMode = allRenderModes[renderModeIndex];
        //if (currentMode == RenderModes.PIXELOLD)
        //    return new Vector2(Instance.renderTexture.width, Instance.renderTexture.height);

        return new Vector2(Screen.width, Screen.height);
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

    public Camera ActiveCamera()
    {
        RenderModes currentMode = allRenderModes[renderModeIndex];
        if (currentMode == RenderModes.RAW)
            return cameraRaw;
        else if (currentMode == RenderModes.PIXELNEW)
            return cameraPixel2;

        return cameraPixel;
    }

    private void UpdateRenderMode(RenderModes renderMode)
    {
        renderModeDisplayText.text = "RenderMode: " + renderMode;

        switch (renderMode)
        {
            case RenderModes.RAW:
                cameraRaw.gameObject.SetActive(true);
                cameraPixel.gameObject.SetActive(false);
                cameraPixel2.gameObject.SetActive(false);
                break;

            //case RenderModes.PIXELOLD:
            //    cameraRaw.gameObject.SetActive(false);
            //    cameraPixel.gameObject.SetActive(true);
            //    cameraPixel2.gameObject.SetActive(false);
            //    break;

            case RenderModes.PIXELNEW:
                cameraRaw.gameObject.SetActive(false);
                cameraPixel.gameObject.SetActive(false);
                cameraPixel2.gameObject.SetActive(true);
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
            float z = transform.position.z;
            transform.position = new Vector3(smooth ? camPosRounded.x : 0, smooth ? camPosRounded.y : 0, z);
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
        //PIXELOLD,
        PIXELNEW,
    }
}
