using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyVariableModule : MonoBehaviour
{
    public EnemyVariable[] Variables;
    private Dictionary<string, EnemyVariable> variableStorage = new Dictionary<string, EnemyVariable>();

    private void Start()
    {
        PreProcessVariables();
    }
    private void PreProcessVariables()
    {
        foreach (EnemyVariable var in Variables)
        {
            switch (var.Type)
            {
                case EnemyVariable.Types.Position:
                    var.worldPosition = transform.TransformPoint(var.localPosition);
                    variableStorage.Add(var.Name, var);
                    break;
            }
        }
    }

    internal Vector2 Read(EnemyVariableReference reference)
    {
        if (variableStorage.ContainsKey(reference.VarName))
            return variableStorage[reference.VarName].worldPosition;

        Debug.LogError($"A vector named {reference.VarName} could not be found on { name }. Please make sure to add one to the VariableModule.");
        return Vector2.zero;
    }



    public static string[] GetVariables()
    {
        List<string> variableNames = new List<string>();
        foreach (EnemyVariableModule module in FindObjectsOfType<EnemyVariableModule>())
        {
            foreach (EnemyVariable var in module.Variables)
            {
                if (!variableNames.Contains(var.Name))
                    variableNames.Add(var.Name);
            }
        }
        return variableNames.ToArray();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (EnemyVariable var in Variables)
        {
            if (var.Type == EnemyVariable.Types.Position)
            {
                Vector2 pos = Application.isPlaying ? var.worldPosition : (Vector2)transform.TransformPoint(var.localPosition);
                Gizmos.DrawSphere(pos, 0.5f);
                Handles.Label(pos + Vector2.up, var.Name);
            }
        }
    }
}
