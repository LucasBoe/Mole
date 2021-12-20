using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IRopeable
{
    float PullForce { get; }
    float Buffer { get; }
    float DistanceDifference { get; }
    float JointDistance { get; }
    bool HasControl { get; }
    RopeConnectionInformation DeactivateAndFetchInfo();
    void ChangeRopeLength(float lengthChange);
}

[RequireComponent(typeof(DistanceJoint2D))]
public class RopeElement : MonoBehaviour, IRopeable
{
    [SerializeField] private RopeConnectionVisualizer visualizerPrefab;
    [SerializeField] private DistanceJoint2D otherJoint;

    [SerializeField] private SpringJoint2D attachJoint;
    [SerializeField] private Rigidbody2D rigidbody2D;

    [SerializeField] private bool shouldOverrideDistance = true;

    private RopeConnectionVisualizer visualizerInstance;
    private float buffer = 0;
    private float pullForce;
    private float realDistance;
    private float jointDistance = 1;

    public float Buffer => buffer;
    public float PullForce => pullForce;
    public float DistanceDifference => jointDistance - realDistance;
    public float JointDistance => jointDistance;
    public bool HasControl => false;

    private float elementLength;
    public float ElementLength { get => elementLength; }

    public Rigidbody2D Rigidbody2D { get => rigidbody2D; }
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

        Vector2 otherAnchor = otherJoint.connectedBody.transform.TransformPoint(otherJoint.connectedAnchor);
        Vector2 ownAnchor = transform.TransformPoint(otherJoint.anchor);
        realDistance = Vector2.Distance(otherAnchor, ownAnchor);
        jointDistance = otherJoint.distance;

        UpdateBuffer();
    }
    public void ChangeRopeLength(float lengthChange)
    {
        if (lengthChange != 0)
        {
            if (lengthChange > 0)
                buffer += lengthChange;
            else
            {
                float rest = TryRemoveFromBuffer(lengthChange);
                otherJoint.distance += rest;
                rigidbody2D.AddForce(Vector2.up);
            }
        }
    }
    public void Setup(Rigidbody2D attached, Rigidbody2D other)
    {
        otherJoint.connectedBody = other;
        otherJoint.connectedAnchor = Vector2.zero;

        attachJoint.connectedBody = attached;
        attachJoint.connectedAnchor = Vector2.zero;
    }
    public RopeConnectionInformation DeactivateAndFetchInfo()
    {
        RopeAnchor anchor = otherJoint.connectedBody.GetComponent<RopeAnchor>();
        RopeConnectionInformation information = new RopeConnectionInformation() { Length = JointDistance, Buffer = Buffer, Anchor = anchor };
        Destroy(visualizerInstance.gameObject);
        Destroy(gameObject);
        return information;
    }
    private void UpdateBuffer()
    {
        while (buffer > 0.01f && (realDistance + 0.01f > jointDistance))
        {
            rigidbody2D.AddForce(Vector2.down);
            otherJoint.distance += 0.01f;
            buffer -= 0.01f;
        }

        if (buffer > 0 && (realDistance + 0.05f > jointDistance))
        {
            otherJoint.distance += buffer;
            buffer = 0;
        }

        visualizerInstance.SetBuffer(buffer);
    }
    private float TryRemoveFromBuffer(float lengthChange)
    {
        if (Mathf.Abs(lengthChange) < buffer)
        {
            buffer += lengthChange;
            return 0f;
        }
        else
        {
            float b = buffer + lengthChange;
            buffer = 0f;

            return b;
        }
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
