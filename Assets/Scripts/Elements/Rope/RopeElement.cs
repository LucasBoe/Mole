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
    [SerializeField] private DistanceJoint2D joint2D;
    [SerializeField] private Rigidbody2D rigidbody2D;

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
    public float ElementLength { get => elementLength;  }
    public Rigidbody2D Rigidbody2D { get => rigidbody2D; }
    public Rigidbody2D TargetRigidbody2D { get => joint2D.connectedBody; }

    internal void SetJointDistance(float newDistance)
    {
        joint2D.distance = newDistance; ;
    }


    // Start is called before the first frame update
    void Start()
    {
        visualizerInstance = Instantiate(visualizerPrefab);
        visualizerInstance.Init(transform, joint2D.connectedBody.transform);
    }

    private void Update()
    {
        pullForce = joint2D.reactionForce.magnitude;

        Vector2 otherAnchor = joint2D.connectedBody.transform.TransformPoint(joint2D.connectedAnchor);
        Vector2 ownAnchor = transform.TransformPoint(joint2D.anchor);
        realDistance = Vector2.Distance(otherAnchor, ownAnchor);
        jointDistance = joint2D.distance;

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
                joint2D.distance += rest;
                rigidbody2D.AddForce(Vector2.up);
            }
        }
    }
    public void Setup(Rigidbody2D toAttachTo, Rigidbody2D toConnectTo)
    {
        float dist = Vector2.Distance(toAttachTo.position, toConnectTo.position);
        joint2D.connectedBody = toConnectTo;
        joint2D.connectedAnchor = Vector2.zero;

        SpringJoint2D fix = GetComponent<SpringJoint2D>();
        fix.connectedBody = toAttachTo;
        fix.connectedAnchor = Vector2.zero;
    }
    public RopeConnectionInformation DeactivateAndFetchInfo()
    {
        RopeAnchor anchor = joint2D.connectedBody.GetComponent<RopeAnchor>();
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
            joint2D.distance += 0.01f;
            buffer -= 0.01f;
        }

        if (buffer > 0 && (realDistance + 0.05f > jointDistance))
        {
            joint2D.distance += buffer;
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

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, pullForce.ToString());
    }
}
