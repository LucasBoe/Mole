using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerHandler : SingletonBehaviour<LayerHandler>
{
    [SerializeField] GameObject indoor, outdoor;
    public GameObject[] LayerGameObjects;

    public static LayerHandler EditorInstance => (Instance == null) ? FindInstance() : Instance;

    private static LayerHandler FindInstance()
    {
        Instance = FindObjectOfType<LayerHandler>();
        return Instance;
    }

    public bool Switch()
    {
        bool indoorActive = indoor.activeSelf;
        indoor.SetActive(!indoorActive);
        outdoor.SetActive(indoorActive);
        return !indoorActive;
    }

    internal void SetIndoorOutdoor(bool isIndoor)
    {
        indoor.SetActive(isIndoor);
        outdoor.SetActive(!isIndoor);
    }
}
