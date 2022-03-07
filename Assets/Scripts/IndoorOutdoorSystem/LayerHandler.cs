using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

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
    public bool HideInactiveLayers = true;

#if UNITY_EDITOR
    public static LayerHandler EditorInstance => (Instance == null) ? FindInstance() : Instance;
    private static LayerHandler FindInstance()
    {
        LayerHandler i = FindObjectOfType<LayerHandler>();
        OverrideInstanceEditor(i);
        return i;
    }

#endif

    public static System.Action<Layers, LayerDataPackage, bool> OnChangeLayer;
    public static Transform Parent => Instance.layerEnumGameobjectPair.Where(l => l.IsActive).Select(l => l.GameObject.transform).First();

    private Layers currentLayer = Layers.Outdoor;
    public Layers CurrentLayer => currentLayer;

    internal void SwitchLayer(Layers newLayer, bool spy = false)
    {
        foreach (LayerDataPackage package in layerEnumGameobjectPair)
        {
            if (package.Layer != newLayer)
            {
                package.GameObject.SetActive(false);
                package.IsActive = false;
            }
        }

        foreach (LayerDataPackage package in layerEnumGameobjectPair)
        {
            if (package.Layer == newLayer)
            {
                OnChangeLayer?.Invoke(currentLayer, package, spy);
                package.GameObject.SetActive(true);
                package.IsActive = true;
                currentLayer = newLayer;
                return;
            }
        }
    }

    public void SwitchLayerEditor(int index)
    {

        Layers newLayer = layerEnumGameobjectPair[index].Layer;

        foreach (LayerDataPackage package in layerEnumGameobjectPair)
        {
            package.GameObject.SetActive(false);
            if (HideInactiveLayers)
            {
                package.GameObject.hideFlags = HideFlags.HideInHierarchy;
                Debug.Log("Hide " + package.GameObject);
            }
            else
            {
                package.GameObject.hideFlags = HideFlags.None;
            }
        }

        foreach (LayerDataPackage package in layerEnumGameobjectPair)
        {
            if (package.Layer == newLayer)
            {
                package.GameObject.SetActive(true);
                package.GameObject.hideFlags = HideFlags.None;
                currentLayer = newLayer;
                return;
            }
        }
    }

    public void ShowAllLayersEditor()
    {
        foreach (LayerDataPackage package in layerEnumGameobjectPair)
        {
            package.GameObject.hideFlags = HideFlags.None;
        }
    }
}

[System.Serializable]
public class LayerDataPackage
{
    public Layers Layer;
    public GameObject GameObject;
    public bool IsTunnel;
    public bool IsActive = false;
}
