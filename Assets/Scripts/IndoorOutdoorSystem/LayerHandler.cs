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

    internal void SetLayer(Layers layerLeadsTo, bool spy = false)
    {
        Layers before = Layers.Outdoor;

        foreach (LayerDataPackage package in layerEnumGameobjectPair)
        {
            if (package.Layer != layerLeadsTo)
            {
                if (package.GameObject.activeSelf)
                    before = package.Layer;

                package.GameObject.SetActive(false);
            }
        }

        foreach (LayerDataPackage package in layerEnumGameobjectPair)
        {
            if (package.Layer == layerLeadsTo)
            {
                OnChangeLayer?.Invoke(before, package, spy);
                package.GameObject.SetActive(true);
                return;
            }
        }
    }

    internal Layers GetCurrentLayer()
    {
        //TODO: acutally implement logic for this one
        return Layers.Outdoor;
    }
}

[System.Serializable]
public class LayerDataPackage
{
    public Layers Layer;
    public GameObject GameObject;
    public bool IsTunnel;
}
