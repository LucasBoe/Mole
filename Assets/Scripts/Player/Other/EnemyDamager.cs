using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamager : MonoBehaviour
{
    [SerializeField] private new Rigidbody2D rigidbody2D;
    [SerializeField] private int damageMultiplier = 10;
    [SerializeField] private int treshhold = 5;
    public bool DoKnock;

    public int FetchDamageAmout()
    {
        Vector2 vel = FetchVelocity();
        if (vel.magnitude > treshhold)
        {
            rigidbody2D.velocity = vel * 0.5f;
            return (int)(vel.magnitude * damageMultiplier);
        }

        return 0;
    }

    public Vector2 FetchVelocity()
    {
        return rigidbody2D.velocity;
    }
}
