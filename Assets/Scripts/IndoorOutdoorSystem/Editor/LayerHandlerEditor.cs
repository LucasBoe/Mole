using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LayerHandler))]
public class LayerHandlerEditor : Editor
{
    static int selectedLayer;

    // draw lines between a chosen game object
    // and a selection of added game objects

    private void OnEnable()
    {
        //SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        SceneView.duringSceneGui -= OnScene;
        SceneView.duringSceneGui += OnScene;
    }

    private static void OnScene(SceneView sceneview)
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(0, 0, 200, 200));

        LayerHandler layerHandler = LayerHandler.EditorInstance;
        List<string> buttonText = new List<string>();

        for (int i = 0; i < layerHandler.LayerGameObjects.Length; i++)
        {
            bool isSelected = i == selectedLayer;
            GameObject gameObject = layerHandler.LayerGameObjects[i];
            buttonText.Add((isSelected ? "> " : "  ") + gameObject.name + (isSelected ? " <" : "  "));
        }

        int newSelectedLayer = GUI.SelectionGrid(new Rect(25, 25, 100, 25 * buttonText.Count), selectedLayer, buttonText.ToArray(), 1);

        if (newSelectedLayer != selectedLayer)
        {
            //ativate / deactivate layer
            for (int i = 0; i < layerHandler.LayerGameObjects.Length; i++)
                layerHandler.LayerGameObjects[i].SetActive(i == newSelectedLayer);

            //try selecting foreground layer
            Transform foregroundChild = layerHandler.LayerGameObjects[newSelectedLayer].transform.Find("Foreground");
            if (foregroundChild != null)
                Selection.activeTransform = foregroundChild;
        }


        selectedLayer = newSelectedLayer;

        GUILayout.EndArea();
        Handles.EndGUI();
    }
}