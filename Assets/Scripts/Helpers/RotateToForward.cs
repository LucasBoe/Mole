using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RotateToForward : MonoBehaviour
{
    Rigidbody2D rigidbody2D;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        transform.right = rigidbody2D.velocity.normalized;
    }
}
