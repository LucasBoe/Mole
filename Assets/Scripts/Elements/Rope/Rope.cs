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

    void ChangeRopeLength(float lengthChange);
}

[RequireComponent(typeof(SpringJoint2D))]
public class Rope : MonoBehaviour, IRopeable
{
    [SerializeField] RopeConnectionVisualizer visualizerPrefab;
    private RopeConnectionVisualizer visualizer;

    private SpringJoint2D joint2D;

    private float pullForce;
    private float realDistance;
    private float jointDistance = 1;

    [SerializeField] private float buffer = 0;
    public float Buffer => buffer;

    public float PullForce => pullForce;
    public float DistanceDifference => jointDistance - realDistance;
    public float JointDistance => jointDistance;

    public bool HasControl => false;


    // Start is called before the first frame update
    void Start()
    {
        joint2D = GetComponent<SpringJoint2D>();
        visualizer = Instantiate(visualizerPrefab);
        visualizer.Init(transform, joint2D.connectedBody.transform);
    }

    private void Update()
    {
        float forceRaw = joint2D.reactionForce.magnitude;
        float lerpValue = 1;// Time.deltaTime;
        pullForce = Mathf.Lerp(pullForce, forceRaw, lerpValue);

        Vector2 otherAnchor = joint2D.connectedBody.transform.TransformPoint(joint2D.connectedAnchor);
        Vector2 ownAnchor = transform.TransformPoint(joint2D.anchor);

        Debug.DrawLine(otherAnchor, ownAnchor, Color.red);

        realDistance = Vector2.Distance(otherAnchor, ownAnchor);
        jointDistance = joint2D.distance;

        while (buffer > 0.01f && (realDistance + 0.01f > jointDistance))
        {
            joint2D.distance += 0.01f;
            buffer -= 0.01f;
        }

        if (buffer > 0 && (realDistance + 0.05f > jointDistance))
        {
            joint2D.distance += buffer;
            buffer = 0;
        }

        visualizer.SetBuffer(buffer);
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
            }
        }
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
        //Gizmos.DrawLine(transform.position, transform.position + (Vector3)joint2D.reactionForce.normalized * (PullForce - joint2D.reactionForce.magnitude));
        Handles.Label(transform.position, ((int)(joint2D.reactionForce).magnitude).ToString() + " => " + (int)PullForce);
    }
}
