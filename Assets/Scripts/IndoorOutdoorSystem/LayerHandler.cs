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
    [SerializeField] LayerDataPackage[] layerEnumGameobjectPair;
    public GameObject[] LayerGameObjects => layerEnumGameobjectPair.Select(p => p.GameObject).ToArray();
    public static LayerHandler EditorInstance => (Instance == null) ? FindInstance() : Instance;
    public static System.Action<Layers, LayerDataPackage, bool> OnChangeLayer;

    private static LayerHandler FindInstance()
    {
        Instance = FindObjectOfType<LayerHandler>();
        return Instance;
    }

    private Layers currentLayer = Layers.Outdoor;
    public Layers CurrentLayer => currentLayer;

    internal void SwitchLayer(Layers newLayer, bool spy = false)
    {
        foreach (LayerDataPackage package in layerEnumGameobjectPair)
        {
            if (package.Layer != newLayer)
            {
                package.GameObject.SetActive(false);
            }
        }

        foreach (LayerDataPackage package in layerEnumGameobjectPair)
        {
            if (package.Layer == newLayer)
            {
                OnChangeLayer?.Invoke(currentLayer, package, spy);
                package.GameObject.SetActive(true);
                currentLayer = newLayer;
                return;
            }
        }

    }
}

[System.Serializable]
public class LayerDataPackage
{
    public Layers Layer;
    public GameObject GameObject;
    public bool IsTunnel;
}
