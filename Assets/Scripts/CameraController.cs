using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : SingletonBehaviour<CameraController>
{
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private Camera camera;

    CinemachineFramingTransposer transposer;

    private void OnEnable()
    {
        transposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    public static Vector2 ScreenToWorldPoint(Vector2 vector2)
    {
        Vector2 remap = new Vector2((vector2.x / Screen.width) * Instance.renderTexture.width, (vector2.y / Screen.height) * Instance.renderTexture.height);
        Vector2 inverted = new Vector2(remap.x, Instance.renderTexture.height - remap.y);
        return Instance.camera.ScreenToWorldPoint(inverted);
    }

    private void Update()
    {
        transposer.m_TrackedObjectOffset = PlayerInputHandler.PlayerInput.VirtualCursorToScreenCenter * new Vector2(4,3);
    }
}
