using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothPiece : MonoBehaviour
{
    [SerializeField] LineRenderer leftRenderer, rightRenderer;
    [SerializeField] FixedJoint2D leftFixed, rightFixed;
    [SerializeField] SpringJoint2D leftSpring, rightSpring;

    private void OnEnable()
    {
        Clothline parent = GetComponentInParent<Clothline>();
        parent.OnFinishInit += ConnectToClothline;
    }

    private void ConnectToClothline(Clothline clothline)
    {
        clothline.OnFinishInit -= ConnectToClothline;

        Rigidbody2D closest = clothline.GetClosestElement(transform.position);
        ConnectToRigidbody(closest);
        foreach (Rigidbody2D childRB in GetComponentsInChildren<Rigidbody2D>()) childRB.simulated = true;
    }

    private void ConnectToRigidbody(Rigidbody2D rigidbody2D)
    {
        leftFixed.connectedBody = rigidbody2D;
        rightFixed.connectedBody = rigidbody2D;
    }

    private void Update()
    {
        leftRenderer.SetPosition(0, leftSpring.transform.InverseTransformPoint((Vector2)transform.position + leftSpring.connectedAnchor));
        leftRenderer.SetPosition(1, leftSpring.anchor);
        rightRenderer.SetPosition(0, rightSpring.transform.InverseTransformPoint((Vector2)transform.position + rightSpring.connectedAnchor));
        rightRenderer.SetPosition(1, rightSpring.anchor);
    }
}
