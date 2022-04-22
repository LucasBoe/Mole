using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRagdollModule : EnemyModule<EnemyRagdollModule>
{
    [SerializeField] EnemyRagdoll ragdollPrefab;

    internal void StartRagdolling()
    {
        EnemyRagdoll ragdoll = Instantiate(ragdollPrefab, transform.position, Quaternion.identity, LayerHandler.Parent);
        ragdoll.Connect(enemyBase);
        gameObject.SetActive(false);
    }
}
