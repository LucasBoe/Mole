using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ChainElement : CableElement
{
    [SerializeField] private DistanceJoint2D distanceJoint2D;


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

    public void Setup(Rigidbody2D start, Rigidbody2D end, float startDistance)
    {
        BasicSetup(start, end);

        distanceJoint2D.connectedBody = end;
        distanceJoint2D.distance = startDistance;

        visualizerInstance.Init(start, end);
    }

    internal void Destroy()
    {
        Destroy(visualizerInstance.gameObject);
        Destroy(gameObject);
        Destroy(this);
    }
}
