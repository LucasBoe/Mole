using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ChainElement : CableElement
{
    [SerializeField] private DistanceJoint2D distanceJoint2D;
    [SerializeField] private CableElementVisualizer visualizerPrefab;
    private CableElementVisualizer visualizerInstance;


    [SerializeField] private float pullForce;
    public float PullForce => pullForce;

    public override void SetJointDistance(float newDistance)
    {
        distanceJoint2D.distance = newDistance;
    }

    private void Update()
    {
        float newPullForce = Mathf.Min(attachJoint.reactionForce.magnitude, Time.time);

        pullForce = attachJoint.reactionForce.y; // newPullForce;
    }

    public void Setup(Rigidbody2D attached, Rigidbody2D other, float startDistance)
    {
        otherRigidbody = other;

        distanceJoint2D.connectedBody = other;
        distanceJoint2D.distance = startDistance;

        attachJoint.connectedBody = attached;
        attachJoint.connectedAnchor = Vector2.zero;
        visualizerInstance = Instantiate(visualizerPrefab, transform.position, Quaternion.identity, LayerHandler.Parent);
        visualizerInstance.Init(attached, other);
    }
    internal void Destroy()
    {
        Destroy(visualizerInstance.gameObject);
        Destroy(gameObject);
        Destroy(this);
    }
}
