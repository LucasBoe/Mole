using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IRopeable
{
    float PullForce { get; }
}

[RequireComponent(typeof(DistanceJoint2D))]
public class RopeElement : MonoBehaviour, IRopeable
{
    [SerializeField] private RopeElementVisualizer visualizerPrefab;

    [SerializeField] private DistanceJoint2D otherJoint;
    [SerializeField] private SpringJoint2D attachJoint;

    [SerializeField] private bool shouldOverrideDistance = true;

    private RopeElementVisualizer visualizerInstance;
    private float pullForce;
    public float PullForce => pullForce;

    public Rigidbody2D Rigidbody2DOther { get => otherJoint.connectedBody; }
    public Rigidbody2D Rigidbody2DAttachedTo { get => attachJoint.connectedBody; }

    internal void SetJointDistance(float newDistance)
    {
        if (shouldOverrideDistance)
            otherJoint.distance = newDistance; ;
    }

    internal void Reconnect(Rigidbody2D to)
    {
        Debug.LogWarning($"reconnected from {attachJoint.connectedBody.name} to {to.name}");
        attachJoint.connectedBody = to;
        visualizerInstance.Init(to, otherJoint.connectedBody);
    }


    // Start is called before the first frame update
    void Start()
    {
        visualizerInstance = Instantiate(visualizerPrefab);
        visualizerInstance.Init(attachJoint.connectedBody, otherJoint.connectedBody);
    }

    private void Update()
    {
        pullForce = otherJoint.reactionForce.magnitude;
    }

    public void Setup(Rigidbody2D attached, Rigidbody2D other)
    {
        otherJoint.connectedBody = other;
        otherJoint.connectedAnchor = Vector2.zero;

        attachJoint.connectedBody = attached;
        attachJoint.connectedAnchor = Vector2.zero;
    }

    internal void FixateDistance(bool active)
    {
        shouldOverrideDistance = active;
        if (!shouldOverrideDistance)
            otherJoint.distance = float.MaxValue;
    }

    private void OnDrawGizmos()
    {
        Handles.Label(Vector3.Lerp(Rigidbody2DOther.position, Rigidbody2DAttachedTo.position, 0.5f), otherJoint.distance.ToString());
    }
}
