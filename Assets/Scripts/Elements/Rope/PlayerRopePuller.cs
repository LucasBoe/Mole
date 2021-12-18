using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerRopePuller : SingletonBehaviour<PlayerRopePuller>, IRopeable
{
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private RopeConnectionVisualizer visualizerPrefab;
    [SerializeField] private DistanceJoint2D joint2D;
    [SerializeField] private RopeAnchor anchor;

    private RopeConnectionVisualizer ropeVisualizer;
    private float pullForceMultiplier = 100;
    private float changeInDistance;
    private Vector2 lastPos;

    public bool IsActive => joint2D.isActiveAndEnabled;
    public bool HasControl => true;
    public float PullForce => changeInDistance * pullForceMultiplier;
    public float DistanceDifference => 0;
    public float JointDistance => Vector2.Distance(transform.position, anchor.transform.position);
    public float Buffer => 0f;

    private void Start()
    {
        ropeVisualizer = Instantiate(visualizerPrefab);
        ropeVisualizer.Init(transform, anchor.transform);
        Invoke("FetchMaxDistance", 0.1f);
    }
    private void FetchMaxDistance() { joint2D.distance = anchor.GetTotalRopeLength(); }

    public RopeConnectionInformation DeactivateAndFetchInfo()
    {
        anchor.ClearSlot(this);
        SetActiveState(false);
        return new RopeConnectionInformation() { Length = Vector2.Distance(anchor.transform.position, transform.position), Anchor = anchor };
    }

    public void Activate(Rope toReplace)
    {
        RopeAnchor.RopeSlot slot = anchor.ClearSlot(toReplace);
        anchor = toReplace.DeactivateAndFetchInfo().Anchor;
        anchor.ConnectRopeToSlot(this, slot);
        SetActiveState(true);
    }

    private void Update()
    {
        Vector2 pos = transform.position;

        if (lastPos != Vector2.zero)
            changeInDistance = Vector2.Distance(lastPos, anchor.transform.position) - Vector2.Distance(pos, anchor.transform.position);

        lastPos = pos;
    }

    public void ChangeRopeLength(float lengthChange) { }

    private void SetActiveState(bool active)
    {
        joint2D.enabled = active;
        ropeVisualizer.gameObject.SetActive(active);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine(transform.position, transform.position + (Vector3)joint2D.reactionForce.normalized * (PullForce - joint2D.reactionForce.magnitude));
        Handles.Label(transform.position, ((int)PullForce).ToString());
    }
}
