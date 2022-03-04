using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatTargetModule : EnemyModule<EnemyCombatTargetModule>, ICombatTarget
{
    [SerializeField] Rigidbody2D rigidbody2D;
    public Rigidbody2D Rigidbody2D => rigidbody2D;

    public Vector2 StranglePosition => Vector2.zero;

    public ICollisionModifier CollisionModifier => GetModule<EnemyRigidbodyControllerModule>();

    public bool IsNull => this == null;

    public void Kill()
    {
        GetModule<EnemyDamageModule>().Kill();
    }

    public bool StartStrangling()
    {
        return false;
    }

    public void StopStrangling(Vector2 playerPos)
    {
        //
    }
}
