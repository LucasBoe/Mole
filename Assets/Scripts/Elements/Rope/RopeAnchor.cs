using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RopeAnchor : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    public Rigidbody2D Rigidbody2D => rigidbody2D;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public Vector2 GetAroundPosition(Vector2 inVector)
    {
        return transform.position.ToVector2() - Vector2.Perpendicular(inVector);
    }
}
