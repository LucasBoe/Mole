using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyBehaviorState
{
    Routine,
    Alert,
    Return,
    Attack,
}

public class SimpleEnemy : MonoBehaviour
{
    [SerializeField] Vector2 eyePosition;
    [SerializeField] float viewConeDistance, viewConeHeight;


    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite passive, active;
    [SerializeField] GameObject indicator;

    [SerializeField] EnemyViewcone viewconeModule;
    [SerializeField] EnemyAIRoutineModule routineModule;
    [SerializeField] EnemyAIMoveModule moveModule;
    [SerializeField] NoiseListener noiseListenerModule;

    [SerializeField] EnemyBehaviorState Current;
    EnemyMemory memory;

    private void Start()
    {
        viewconeModule.OnPlayerEntered += AlertMoveTo;
        noiseListenerModule.OnNoise += AlertMoveTo;
        SetState(EnemyBehaviorState.Routine);
    }

    private void SetState(EnemyBehaviorState behaviorState)
    {
        if (Current == EnemyBehaviorState.Routine)
            routineModule.StopRoutine();
        else
            StopAllCoroutines();

        SetIndicatorActive(false);

        Current = behaviorState;

        switch (Current)
        {
            case EnemyBehaviorState.Routine:
                routineModule.StartRoutine();
                break;

            case EnemyBehaviorState.Alert:
                StartCoroutine(AlertRoutine());
                moveModule.MoveTo(memory.target, ReachedTarget);
                break;
        }
    }

    public void AlertMoveTo(Vector2 pos)
    {
        memory.target = pos;
        SetState(EnemyBehaviorState.Alert);
    }

    public void ReachedTarget ()
    {
        SetState(EnemyBehaviorState.Routine);
    }


    private IEnumerator AlertRoutine()
    {
        bool _switch = true;
        while (true)
        {
            SetIndicatorActive(_switch);
            _switch = !_switch;
            yield return new WaitForSeconds(0.5f);
        }
    }
    internal void UpdateViewcone()
    {
        viewconeModule.UpdateBounds(eyePosition, viewConeDistance, viewConeHeight);
    }
    private void SetIndicatorActive(bool act)
    {
        spriteRenderer.sprite = act ? active : passive;
        indicator.SetActive(act);
    }
}

public struct EnemyMemory
{
    public Vector2 target;
}
