using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IRopeable
{
    float PullForce { get; }
    float DistanceDifference { get; }
    float JointDistance { get; }

    void ChangeRopeLength(float lengthChange);
}

[RequireComponent(typeof(SpringJoint2D))]
public class Rope : MonoBehaviour, IRopeable
{
    [SerializeField] RopeConnectionVisualizer visualizerPrefab;

    private SpringJoint2D joint2D;

    private float pullForce;
    private float realDistance;
    private float jointDistance = 1;

    public float PullForce => pullForce;
    public float DistanceDifference => jointDistance - realDistance;
    public float JointDistance => jointDistance;

    // Start is called before the first frame update
    void Start()
    {
        joint2D = GetComponent<SpringJoint2D>();
        Instantiate(visualizerPrefab).Init(transform, joint2D.connectedBody.transform);
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

    }

    public void ChangeRopeLength(float lengthChange)
    {
        joint2D.distance += lengthChange;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine(transform.position, transform.position + (Vector3)joint2D.reactionForce.normalized * (PullForce - joint2D.reactionForce.magnitude));
        Handles.Label(transform.position, ((int)(joint2D.reactionForce).magnitude).ToString() + " => " + (int)PullForce);
    }
}
