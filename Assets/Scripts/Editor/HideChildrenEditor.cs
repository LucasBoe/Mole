using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(HideChildren))]
[CanEditMultipleObjects]
public class HideChildrenEditor : Editor
{
    HideChildren hider;

    private void OnEnable()
    {
        hider = (target as HideChildren);
        hider.UpdateHideFlags();
    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        if (hider.HasChildren)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(hider.transform.childCount.ToString() + " children ( " + (hider.Hide ? "HIDDEN" : "VISIBLE") + " )");
            Texture2D texture = EditorGUIUtility.FindTexture(hider.Hide ? "animationvisibilitytoggleon" : "animationvisibilitytoggleoff");
            GUIContent content = new GUIContent(hider.Hide ? "Show Children" : "Hide Children", texture);
            if (GUILayout.Button(content))
            {
                hider.SetHide(!hider.Hide);
                hider.UpdateHideFlags();
            }

            if (GUILayout.Button("U", GUILayout.Width(EditorGUIUtility.singleLineHeight)))
                hider.UpdateHideFlags();
            GUILayout.EndHorizontal();
        }
    }
}
