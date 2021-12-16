using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlertVisualization : MonoBehaviour
{
    EnemyStatemachineModule stateMachine;
    [SerializeField] MeshRenderer meshRenderer;

    private void Start()
    {
        stateMachine = GetComponentInParent<EnemyStatemachineModule>();
        stateMachine.OnEnterNewState += OnEnterNewState;
    }

    private void OnEnterNewState(Type type)
    {
        meshRenderer.enabled = type != typeof(EnemyAIRoutineState);
    }
}
