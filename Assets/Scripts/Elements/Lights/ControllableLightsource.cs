using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(LightTrigger), typeof(Light2D))]
public class ControllableLightsource : MonoBehaviour
{
    [SerializeField] private Collider2D lightTrigger;
    private new Light2D light;
    public bool IsOn = true;

    private void Start()
    {
        light = GetComponent<Light2D>();
        SetOn(IsOn);
    }

    internal void Switch()
    {
        SetOn(!IsOn);
    }

    private void SetOn(bool on)
    {
        IsOn = on;
        lightTrigger.enabled = on;
        light.enabled = on;
    }
}
