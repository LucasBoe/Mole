using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LayerHandler))]
public class LayerHandlerEditor : Editor
{
    static int selectedLayer;
    static string selectedGround = "Foreground";

    // draw lines between a chosen game object
    // and a selection of added game objects

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        SceneView.duringSceneGui -= OnScene;
        SceneView.duringSceneGui += OnScene;
    }

    private static void OnScene(SceneView sceneview)
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(0, 0, 200, 200));

        bool update = false;

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

            update = true;


        }

        selectedLayer = newSelectedLayer;

        GUILayout.EndArea();

        if (GUI.Button(new Rect(Screen.width / 2 - 100,25,100,25), selectedGround))
        {
            if (selectedGround == "Foreground")
                selectedGround = "Background";
            else
                selectedGround = "Foreground";

            update = true;
        }

        if (update)
        {
            //try selecting active layer
            Transform foregroundChild = layerHandler.LayerGameObjects[selectedLayer].transform.Find(selectedGround);
            if (foregroundChild != null)
                Selection.activeTransform = foregroundChild;
        }

        Handles.EndGUI();
    }
}