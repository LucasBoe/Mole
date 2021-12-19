using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionProgressionVisualizerUI : TemporaryUIElement
{
    Transform player;
    [SerializeField] Image image;

    private void Start()
    {
        GetComponent<TransformTrackingUI>().ToTrack = FindObjectOfType<PlayerController>().transform;
    }

    public void UpdaValue(float value)
    {
        image.fillAmount = value;
    }
}
