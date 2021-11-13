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

    public void PlayerEnteredViewcone(Collider2D player)
    {
        transform.position = player.transform.position;
    }

    internal void UpdateViewcone()
    {
        EnemyViewcone.UpdateBounds(eyePosition, viewConeDistance, viewConeHeight);
    }
}
