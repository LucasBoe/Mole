using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TargetJoint2D))]
public class TargetJointTargetFromTransform : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    TargetJoint2D targetJoint2d;

    // Start is called before the first frame update
    void Start()
    {
        targetJoint2d = GetComponent<TargetJoint2D>();
    }

    // Update is called once per frame
    void Update()
    {
        targetJoint2d.target = targetTransform.position;
    }
}
