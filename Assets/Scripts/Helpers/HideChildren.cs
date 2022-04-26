using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

public class HideChildren : MonoBehaviour
{
    public bool HasChildren => transform.childCount > 0;
    [SerializeField, ReadOnly] private bool hide = false;
    public bool Hide => hide;

    private void Awake()
    {
        Destroy(this);
    }

    private void OnValidate()
    {
        UpdateHideFlags();
    }

    public void UpdateHideFlags()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.hideFlags = Hide ? HideFlags.HideInHierarchy : HideFlags.None;
        }

        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        EditorApplication.RepaintHierarchyWindow();
    }

    public void SetHide(bool h)
    {
        hide = h;
        PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        var prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
        if (prefabStage != null)
        {
            EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
    }
}
