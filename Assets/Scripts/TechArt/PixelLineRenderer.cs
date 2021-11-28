using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelLineRenderer : MonoBehaviour
{
    public PixelLine Line;

    private void Start()
    {
        PixelLineDrawer.Instance.Register(Line);
    }

    private void OnDestroy()
    {
        PixelLineDrawer.Instance.Unregister(Line);
    }
}
