using UnityEngine;
using System.Collections;
using UnityEditor;

[InitializeOnLoad]
public class SceneContextMenu : Editor
{
    static SceneContextMenu()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sceneview)
    {
        if (Event.current.button == 1)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                GenericMenu menu = new GenericMenu();
                Vector3 mousePosition = Event.current.mousePosition;
                mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
                mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);
                menu.AddItem(new GUIContent("Teleport Player Here"), false, TeleportPlayer, mousePosition);
                //menu.AddItem(new GUIContent("Item 2"), false, Callback, 2);
                menu.ShowAsContext();
            }
        }
    }

    static void TeleportPlayer(object obj)
    {
        PlayerController player = PlayerController.Instance;

        if (player == null) return;

        Vector3 pos = (Vector3)obj;
        pos.z = 0;

        player.Teleport(pos);
    }
}