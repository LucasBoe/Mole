using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTrackingUI : MonoBehaviour
{
    public Transform ToTrack;

    private void Update()
    {
        transform.position = CameraController.WorldToScreenPoint(ToTrack.position);
    }
}
