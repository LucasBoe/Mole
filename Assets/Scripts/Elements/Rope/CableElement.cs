using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableElement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D ownRigidbody;
    public Rigidbody2D OwnRigidbody => ownRigidbody;
    [SerializeField] protected AnchoredJoint2D attachJoint;
    [SerializeField] private CableElementVisualizer visualizerPrefab;
    protected CableElementVisualizer visualizerInstance;

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
    protected virtual void BasicSetup(Rigidbody2D start, Rigidbody2D end)
    {
        otherRigidbody = end;
        attachJoint.connectedBody = start;
        attachJoint.connectedAnchor = Vector2.zero;
        visualizerInstance = Instantiate(visualizerPrefab, transform.position, Quaternion.identity, LayerHandler.Parent);
    }
    public void Destroy()
    {
        Destroy(visualizerInstance.gameObject);
        Destroy(gameObject);
        Destroy(this);
    }
}
