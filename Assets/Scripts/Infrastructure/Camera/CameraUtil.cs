using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraUtil 
{
    public static Vector2 ScreenToWorldPoint(Vector2 vector2)
    {
        return CameraController.Instance.ActiveCamera().ScreenToWorldPoint(vector2);
    }

    internal static Vector3 WorldToScreenPoint(Vector3 worldPos)
    {
        return CameraController.Instance.ActiveCamera().WorldToScreenPoint(worldPos);
    }

    public static void SetCameraSize(float newSize)
    {
        CameraController.Instance.SetCameraSize(newSize);
    }

    public static void ResetCameraSize()
    {
        CameraController.Instance.SetCameraSize(CameraController.DEFAULT_CAMERA_SIZE);
    }
}
