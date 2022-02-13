using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RopeElement : MonoBehaviour, IInputActionProvider
{
    [SerializeField] private RopeElementVisualizer visualizerPrefab;
    private RopeElementVisualizer visualizerInstance;

    [SerializeField] private RopeElementPhysicsBehaviour physicsInstance;

    [SerializeField] private AnchoredJoint2D attachJoint;
    public float DistanceToAttachedObject => Vector2.Distance(transform.position, attachJoint.connectedBody.position);




    [SerializeField] private float pullForce;
    public float PullForce => pullForce;


    private Rigidbody2D otherRigidbody;
    public Rigidbody2D Rigidbody2DOther { get => otherRigidbody; }
    public Rigidbody2D Rigidbody2DAttachedTo { get => attachJoint.connectedBody; }

    public void SetJointDistance(float newDistance)
    {
        physicsInstance.SetLength(newDistance);
    }

    public void Reconnect(Rigidbody2D to)
    {
        Debug.Log($"reconnected from {attachJoint.connectedBody.name} to {to.name}");
        attachJoint.connectedBody = to;
        visualizerInstance.Init(to, otherRigidbody, physicsInstance);
    }

    private void Update()
    {
        float newPullForce = Mathf.Min(attachJoint.reactionForce.magnitude, Time.time);

        pullForce = attachJoint.reactionForce.magnitude; // newPullForce;
    }

    public void Setup(Rigidbody2D attached, Rigidbody2D other, float length, Vector2[] travelPoints = null)
    {
        otherRigidbody = other;

        attachJoint.connectedBody = attached;
        attachJoint.connectedAnchor = Vector2.zero;
        if (travelPoints == null)
            physicsInstance.Init(attached, otherRigidbody, length);
        else
            physicsInstance.Init(attached, otherRigidbody, length, travelPoints);

        visualizerInstance = Instantiate(visualizerPrefab, LayerHandler.Parent);
        visualizerInstance.Init(attachJoint.connectedBody, otherRigidbody, physicsInstance);
    }
    internal void Destroy()
    {
        Destroy(physicsInstance.gameObject);
        Destroy(visualizerInstance.gameObject);
        Destroy(gameObject);
        Destroy(this);
    }

    public InputAction FetchInputAction()
    {
        return PlayerInputActionCreator.GetClimbRopeAction(transform);
    }
}
