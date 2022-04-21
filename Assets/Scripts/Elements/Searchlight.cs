using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class Searchlight : MonoBehaviour
{
    [SerializeField] float minAngle, maxAngle;
    [SerializeField] Transform lightTransform;
    [SerializeField] bool active;
    [SerializeField] AnimationCurve angleLerpCurve;
    [SerializeField] float angleOffset = -90f;
    [SerializeField] float angleSpeed = 0.01f;
    [SerializeField] EnemyObervationAreaTrigger obervationAreaTrigger;

    private float t = 0f;
    private List<EnemyBase> enemysInTrigger = new List<EnemyBase>();

    private void Start()
    {
        lightTransform.rotation = RotationFromAngle(minAngle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsEnemy())
        {
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            enemy.GetModule<EnemyPlayerDetectionModule>().RegisterObvervationArea(obervationAreaTrigger);

            enemysInTrigger.Add(enemy);
            UpdateActiveState();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.IsEnemy())
        {
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            enemy.GetModule<EnemyPlayerDetectionModule>().UnregisterObvervationArea(obervationAreaTrigger);

            enemysInTrigger.Remove(enemy);
            UpdateActiveState();
        }
    }

    private void UpdateActiveState()
    {
        active = (enemysInTrigger.Count > 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Debug.DrawLine(lightTransform.position, lightTransform.position + (RotationFromAngle(minAngle) * Vector3.right));
        Debug.DrawLine(lightTransform.position, lightTransform.position + (RotationFromAngle(maxAngle) * Vector3.right));
    }

    private Quaternion RotationFromAngle(float angle)
    {
        return Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        if (!active) return;

        if (!IsControlledByAnyEnemy()) return;

        t += Time.deltaTime * Mathf.Abs(Mathf.DeltaAngle(minAngle, maxAngle)) * angleSpeed;
        float lerp = angleLerpCurve.Evaluate(t);
        float angle = Mathf.LerpAngle(minAngle, maxAngle, lerp) + angleOffset;
        lightTransform.rotation = RotationFromAngle(angle);
    }

    private bool IsControlledByAnyEnemy()
    {
        foreach (EnemyBase enemy in enemysInTrigger)
        {
            BehaviourTreeRunner runner = enemy.GetModule<BehaviourTreeRunner>();
            if (runner.LastNode != null && runner.LastNode.CanUseObjects)
                return true;
        }

        return false;
    }
}
