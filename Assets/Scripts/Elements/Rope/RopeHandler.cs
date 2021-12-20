using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeHandler : SingletonBehaviour<RopeHandler>
{
    [SerializeField] RopeElement ropePrefab;

    private List<Rope> ropes = new List<Rope>();

    public RopeElement CreateRope(Rigidbody2D toAttachTo, Rigidbody2D toConnectTo)
    {
        RopeElement instance = Instantiate(ropePrefab, toAttachTo.position, Quaternion.identity);
        instance.Setup(toAttachTo, toConnectTo);
        return instance;
    }

    internal void CreateRope(Rigidbody2D start, RopeAnchor[] anchors, Rigidbody2D end)
    {
        Rope newRope = new Rope(start, anchors, end);
        ropes.Add(newRope);
    }

    internal RopeElement CreateRopeElement(Rigidbody2D start, Rigidbody2D end)
    {
        RopeElement instance = Instantiate(ropePrefab, start.position, Quaternion.identity);
        instance.Setup(start, end);
        return instance;
    }

    public RopeElement CreateRope(RopeConnectionInformation info)
    {
        RopeAnchor anchor = info.Anchor;
        RopeElement newRope = CreateRope(info.attached, anchor.Rigidbody2D);
        RopeAnchor.RopeSlot slot = anchor.GetEmptySlot();
        anchor.ConnectRopeToSlot(newRope, slot);

        return newRope;
    }

    public RopeElement AttachPlayerRope(Rigidbody2D rigidbody2D)
    {
        RopeConnectionInformation info = PlayerRopePuller.Instance.DeactivateAndFetchInfo();
        info.attached = rigidbody2D;
        return CreateRope(info);
    }

    internal RopeAnchor GetAnchorOf(IRopeable rope)
    {
        foreach (RopeAnchor anchor in FindObjectsOfType<RopeAnchor>())
        {
            if (anchor.HasRope(rope))
                return anchor;
        }

        return null;
    }

    private void Update()
    {
        foreach (Rope rope in ropes)
            rope.Update();
    }
}
