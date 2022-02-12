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

    //[SerializeField] EdgeCollider2D ropeCollider;

    [SerializeField] private bool shouldOverrideDistance = true;

    private float pullForce;
    public float PullForce => pullForce;


    private Rigidbody2D otherRigidbody;
    public Rigidbody2D Rigidbody2DOther { get => otherRigidbody; }
    public Rigidbody2D Rigidbody2DAttachedTo { get => attachJoint.connectedBody; }

    public void SetJointDistance(float newDistance)
    {
        //if (shouldOverrideDistance)
        //    otherJoint.distance = newDistance;
        physicsInstance.SetLength(newDistance);
    }

    public void Reconnect(Rigidbody2D to)
    {
        Debug.LogWarning($"reconnected from {attachJoint.connectedBody.name} to {to.name}");
        attachJoint.connectedBody = to;
        visualizerInstance.Init(to, otherRigidbody, physicsInstance);
    }

    private void Update()
    {
        /*
        ropeCollider.SetPoints(new List<Vector2>(new Vector2[]
        {
            transform.InverseTransformPoint(Rigidbody2DAttachedTo.position),
            transform.InverseTransformPoint(Rigidbody2DOther.position)
        }));
        */


        pullForce = Mathf.Min(attachJoint.reactionForce.magnitude, 25, Time.time);
    }

    public void Setup(Rigidbody2D attached, Rigidbody2D other)
    {
        otherRigidbody = other;

        attachJoint.connectedBody = attached;
        attachJoint.connectedAnchor = Vector2.zero;
        physicsInstance.Init(attached, otherRigidbody);

        visualizerInstance = Instantiate(visualizerPrefab);
        visualizerInstance.Init(attachJoint.connectedBody, otherRigidbody, physicsInstance);
    }

    internal void FixateDistance(bool active)
    {
        shouldOverrideDistance = active;
        //if (!shouldOverrideDistance)
        //    otherJoint.distance = float.MaxValue;
    }

    public Vector2 GetClosestPoint(Vector2 point)
    {
        if (!otherRigidbody || !attachJoint)
            return point;
        else
            return Util.GetClosestPointOnLineSegment(Rigidbody2DOther.position, Rigidbody2DAttachedTo.position, point);
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
        return new InputAction() { ActionCallback = Climb, Stage = InputActionStage.WorldObject, Target = gameObject.AddComponent<SpriteRenderer>(), Text = "Climb" };
    }

    private void Climb()
    {
        PlayerStateMachine.Instance.SetState(new RopeClimbState(this));
    }
}
