using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePhysicsSegment : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D capsuleCollider;
    [SerializeField] private HingeJoint2D joint;
    [SerializeField] private new Rigidbody2D rigidbody;
    public Rigidbody2D Rigidbody => rigidbody;

    public void Connected(Rigidbody2D connect)
    {
        joint.connectedBody = connect;
    }

    internal void SetDistance(float length)
    {
        joint.anchor = new Vector2(-length / 2, 0);
        joint.connectedAnchor = new Vector2(0.5f, 0);
        capsuleCollider.size = new Vector2(length, 0.1f);
    }

    internal Vector2 GetEnd()
    {
        return rigidbody.position + transform.right.ToVector2() * capsuleCollider.size.x / 2;
    }
}
