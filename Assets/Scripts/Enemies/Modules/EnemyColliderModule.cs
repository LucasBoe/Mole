using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColliderModule : EnemyModule<EnemyColliderModule>, ICollisionModifier
{
    [SerializeField] private Collider2D collider2D;

    public void SetCollisionActive(bool active)
    {
        collider2D.enabled = active;
    }
}
