using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerTrigger : PlayerTriggerBehaviour
{
    [SerializeField] CircleCollider2D circleCollider2D;
    public System.Action<Collider2D> PlayerEnter, PlayerExit;
    public EnemyPlayerTrigger Init(float size)
    {
        circleCollider2D.radius = size;
        return this;
    }

    protected override void OnPlayerEnter2D(Collider2D playerCollider)
    {
        PlayerEnter?.Invoke(playerCollider);
    }

    protected override void OnPlayerExit2D(Collider2D playerCollider)
    {
        PlayerExit?.Invoke(playerCollider);
    }
}
