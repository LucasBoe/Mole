using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlertVisualization : MonoBehaviour
{
    EnemyStatemachineModule stateMachine;
    [SerializeField] SpriteRenderer renderer;

    private void Start()
    {
        stateMachine = GetComponentInParent<EnemyStatemachineModule>();
        stateMachine.OnEnterNewState += OnEnterNewState;
    }

    private void OnEnterNewState(Type type)
    {
        renderer.enabled = type != typeof(EnemyAIRoutineState);
    }
}
