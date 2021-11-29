using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite passive, active;
    [SerializeField] GameObject indicator;

    SimpleEnemy enemy;

    private void Start()
    {
        enemy = GetComponent<SimpleEnemy>();
        enemy.OnStateChange += OnStateChange;
    }

    private void OnStateChange(EnemyStateType state)
    {
        StopAllCoroutines();
        SetIndicatorActive(false);

        if (state == EnemyStateType.Alert)
            StartCoroutine(AlertRoutine());
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
