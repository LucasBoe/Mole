using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPositionTrackingUI : MonoBehaviour
{
    public Vector3 WorldPosition;

    private void Update()
    {
        transform.position = CameraController.WorldToScreenPoint(WorldPosition);
    }
}
