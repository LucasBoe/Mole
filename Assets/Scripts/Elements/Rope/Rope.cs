using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(SpringJoint2D))]
public class Rope : MonoBehaviour
{
    [SerializeField] RopeAnchor anchor;

    SpringJoint2D joint2D;

    public float PullForce;
    public float RealDistance;
    public float JointDistance;

    string log = "dist \n";
    string log2 = "distReal \n";

    // Start is called before the first frame update
    void Start()
    {
        joint2D = GetComponent<SpringJoint2D>();
    }

    private void Update()
    {
        //joint2D.distance = Mathf.MoveTowards(joint2D.distance, targetDistance, Time.deltaTime);
        float forceRaw = joint2D.reactionForce.magnitude;
        float lerpValue = 1;// Time.deltaTime;
        PullForce = Mathf.Lerp(PullForce, forceRaw, lerpValue);

        Vector2 otherAnchor = joint2D.connectedBody.transform.TransformPoint(joint2D.connectedAnchor);
        //Vector2 ownAnchor = transform.TransformPoint(joint2D.anchor);
        Vector2 ownAnchor = joint2D.attachedRigidbody.ClosestPoint(otherAnchor);
        RealDistance = Vector2.Distance(otherAnchor, ownAnchor);

    }

    public void ChangeRopeDistance(float newDistance)
    {
        JointDistance = newDistance;
        joint2D.distance = newDistance;
        //Vector2 pos = joint2D.connectedBody.transform.TransformPoint(joint2D.connectedAnchor);
        //Vector2 pos2 = transform.TransformPoint(joint2D.anchor);
        //float distanceReal = Vector2.Distance(pos, pos2);
        //log += joint2D.distance + " \n";
        //log2 += distanceReal + " \n";
        //Debug.LogWarning(log);
        //Debug.LogWarning(log2);
    }

    public void ChangeRopeLength(float lengthChange)
    {
        joint2D.distance += lengthChange;
        JointDistance = joint2D.distance;
        //Vector2 pos = joint2D.connectedBody.transform.TransformPoint(joint2D.connectedAnchor);
        //Vector2 pos2 = transform.TransformPoint(joint2D.anchor);
        //float distanceReal = Vector2.Distance(pos, pos2);
        //log += joint2D.distance + " \n";
        //log2 += distanceReal + " \n";
        //Debug.LogWarning(log);
        //Debug.LogWarning(log2);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine(transform.position, transform.position + (Vector3)joint2D.reactionForce.normalized * (PullForce - joint2D.reactionForce.magnitude));
        //Handles.Label(transform.position, ((int)(joint2D.reactionForce).magnitude).ToString() + " => " + (int)PullForce);
    }

}
