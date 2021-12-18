using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerRopePuller : SingletonBehaviour<PlayerRopePuller>, IRopeable
{
    [SerializeField] RopeConnectionVisualizer visualizerPrefab;
    [SerializeField] DistanceJoint2D joint2D;
    [SerializeField] RopeAnchor anchor;
    [SerializeField] float pullForceMultiplier = 1;

    Rigidbody2D rigidbody2D;
    RopeConnectionVisualizer ropeVisualizer;

    private float changeInDistance;
    private Vector2 lastPos;

    public RopeConnectionInformation DeactivateAndFetchInfo()
    {
        anchor.ClearSlot(this);
        joint2D.enabled = false;
        ropeVisualizer.gameObject.SetActive(false);

        return new RopeConnectionInformation() { Length = Vector2.Distance(anchor.transform.position, transform.position), Anchor = anchor };
    }

    public void ReplaceRope(Rope connected)
    {
        RopeAnchor.RopeSlot slot = anchor.ClearSlot(connected);

        RopeConnectionInformation info = connected.DeactivateAndFetchInfo();
        anchor = info.Anchor;

        anchor.ConnectRopeToSlot(this, slot);
        joint2D.enabled = true;
        ropeVisualizer.gameObject.SetActive(true);
    }

    public bool IsActive => joint2D.isActiveAndEnabled;

    public bool HasControl => true;
    public float PullForce => changeInDistance * pullForceMultiplier;

    public float DistanceDifference => 0;

    public float JointDistance => Vector2.Distance(transform.position, anchor.transform.position);

    public float Buffer => 0f;

    private void Update()
    {

        Vector2 pos = transform.position;

        if (lastPos != Vector2.zero)
            changeInDistance = Vector2.Distance(lastPos, anchor.transform.position) - Vector2.Distance(pos, anchor.transform.position);


        lastPos = pos;
    }

    private void Start()
    {
        rigidbody2D = GetComponentInParent<Rigidbody2D>();
        ropeVisualizer = Instantiate(visualizerPrefab);
        ropeVisualizer.Init(transform, anchor.transform);
        Invoke("FetchMaxDistance", 0.1f);
    }

    public void ChangeRopeLength(float lengthChange)
    {

    }

    private void FetchMaxDistance()
    {
        joint2D.distance = anchor.GetTotalRopeLength();
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine(transform.position, transform.position + (Vector3)joint2D.reactionForce.normalized * (PullForce - joint2D.reactionForce.magnitude));
        Handles.Label(transform.position, ((int)PullForce).ToString());
    }
}
