using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    void PlayerEnteredViewcone(Collider2D player);
}

public class SimpleEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] Vector2 eyePosition;
    [SerializeField] float viewConeDistance, viewConeHeight;

    [SerializeField] EnemyViewcone EnemyViewcone;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite passive, active;
    [SerializeField] GameObject indicator;

    [SerializeField] EnemyAIRoutineModule routineModule;
    [SerializeField] EnemyAIMoveModule moveModule;

    private void Start()
    {
        StopAllCoroutines();
        routineModule.StartRoutine();
    }

    public void PlayerEnteredViewcone(Collider2D player)
    {
        routineModule.StopRoutine();
        moveModule.MoveTo(player.transform.position, Start);
        StartCoroutine(AlertRoutine());
    }

    internal void UpdateViewcone()
    {
        EnemyViewcone.UpdateBounds(eyePosition, viewConeDistance, viewConeHeight);
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


    private void SetIndicatorActive(bool act)
    {
        spriteRenderer.sprite = act ? active : passive;
        indicator.SetActive(act);
    }
}
