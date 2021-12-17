using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRopePuller : MonoBehaviour, IRopeable
{
    [SerializeField] RopeConnectionVisualizer visualizerPrefab;
    [SerializeField] RopeAnchor anchor;
    [SerializeField] float pullForceMultiplier = 1;

    Rigidbody2D rigidbody2D;
    RopeConnectionVisualizer ropeVisualizer;

    private void Start()
    {
        rigidbody2D = GetComponentInParent<Rigidbody2D>();
        ropeVisualizer = Instantiate(visualizerPrefab);
        ropeVisualizer.Init(transform, anchor.transform);
    }

    public float PullForce => rigidbody2D.velocity.magnitude * pullForceMultiplier;

    public float DistanceDifference => 0;

    public float JointDistance => Vector2.Distance(transform.position, anchor.transform.position);

    public void ChangeRopeLength(float lengthChange)
    {
        //
    }
}
