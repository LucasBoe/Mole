using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Layers
{
    Indoor,
    Outdoor,
    Tunnels,
}

public class LayerHandler : SingletonBehaviour<LayerHandler>
{
    [SerializeField] GameObject indoor, outdoor;
    [SerializeField] LayerEnumGameobjectPair[] layerEnumGameobjectPair;
    public GameObject[] LayerGameObjects => layerEnumGameobjectPair.Select(p => p.GameObject).ToArray();

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

    internal void SetLayer(Layers layerLeadsTo)
    {
        foreach (LayerEnumGameobjectPair pair in layerEnumGameobjectPair)
        {
            if (pair.Layer != layerLeadsTo)
            {
                pair.GameObject.SetActive(false);
            }
        }

        foreach (LayerEnumGameobjectPair pair in layerEnumGameobjectPair)
        {
            if (pair.Layer == layerLeadsTo)
            {
                pair.GameObject.SetActive(true);
            }
        }
    }

    internal void SetIndoorOutdoor(bool isIndoor)
    {
        indoor.SetActive(isIndoor);
        outdoor.SetActive(!isIndoor);
    }
}

[System.Serializable]
public class LayerEnumGameobjectPair
{
    public Layers Layer;
    public GameObject GameObject;
}
