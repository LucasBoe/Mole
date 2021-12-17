using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerRopePuller : MonoBehaviour, IRopeable
{
    [SerializeField] RopeConnectionVisualizer visualizerPrefab;
    [SerializeField] DistanceJoint2D joint2D;
    [SerializeField] RopeAnchor anchor;
    [SerializeField] float pullForceMultiplier = 1;

    Rigidbody2D rigidbody2D;
    RopeConnectionVisualizer ropeVisualizer;

    private float changeInDistance;
    private Vector2 lastPos;

    public bool HasControl => true;
    public float PullForce => changeInDistance * pullForceMultiplier;

    public float DistanceDifference => 0;

    public float JointDistance => Vector2.Distance(transform.position, anchor.transform.position);

    private void Update()
    {

        Vector2 pos = transform.position;

        if (lastPos != Vector2.zero)
            changeInDistance = Vector2.Distance(lastPos, anchor.transform.position) - Vector2.Distance(pos, anchor.transform.position);

        joint2D.distance = anchor.GetTotalRopeLength();

        lastPos = pos;
    }

    private void Start()
    {
        rigidbody2D = GetComponentInParent<Rigidbody2D>();
        ropeVisualizer = Instantiate(visualizerPrefab);
        ropeVisualizer.Init(transform, anchor.transform);
    }

    public void ChangeRopeLength(float lengthChange)
    {

    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine(transform.position, transform.position + (Vector3)joint2D.reactionForce.normalized * (PullForce - joint2D.reactionForce.magnitude));
        Handles.Label(transform.position, ((int)PullForce).ToString());
    }
}
