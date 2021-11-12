using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutdoorIndoorHandler : SingletonBehaviour<OutdoorIndoorHandler>
{
    [SerializeField] GameObject indoor, outdoor;

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
