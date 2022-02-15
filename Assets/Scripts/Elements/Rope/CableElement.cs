using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableElement : MonoBehaviour
{
    [SerializeField] protected AnchoredJoint2D attachJoint;

    protected Rigidbody2D otherRigidbody;
    public Rigidbody2D Rigidbody2DOther { get => otherRigidbody; }
    public Rigidbody2D Rigidbody2DAttachedTo { get => attachJoint.connectedBody; }
    public virtual void SetJointDistance(float newDistance)
    {
        //
    }

    public virtual void Reconnect(Rigidbody2D to)
    {
        //
    }
}
