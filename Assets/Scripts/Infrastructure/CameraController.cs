using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : SingletonBehaviour<CameraController>
{
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private new Camera camera;

    CinemachineVirtualCamera virtualCamera;
    CinemachineFramingTransposer transposer;

    private void OnEnable()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        PlayerController.OnPlayerSpawned += ConnectPlayerTransformToVirtualCamera;
    }

    private void ConnectPlayerTransformToVirtualCamera(Transform player)
    {
        virtualCamera.Follow = player;
    }

    public static Vector2 ScreenToWorldPoint(Vector2 vector2)
    {
        Vector2 remap = new Vector2((vector2.x / Screen.width) * Instance.renderTexture.width, (vector2.y / Screen.height) * Instance.renderTexture.height);
        return Instance.camera.ScreenToWorldPoint(remap);
    }

    internal static Vector3 WorldToScreenPoint(Vector3 worldPos)
    {
        Vector2 raw = Instance.camera.WorldToScreenPoint(worldPos);
        return new Vector2((raw.x / Instance.renderTexture.width) * Screen.width, (raw.y / Instance.renderTexture.height) * Screen.height);
    }

    private void Update()
    {
        transposer.m_TrackedObjectOffset = PlayerInputHandler.PlayerInput.VirtualCursorToScreenCenter * new Vector2(4,3);
    }
}
