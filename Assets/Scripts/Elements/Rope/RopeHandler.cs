using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeHandler : SingletonBehaviour<RopeHandler>
{
    [SerializeField] Rope ropePrefab;

    public Rope CreateRope(Rigidbody2D toAttachTo, Rigidbody2D toConnectTo)
    {
        Rope instance = Instantiate(ropePrefab, toAttachTo.position, Quaternion.identity);
        instance.Setup(toAttachTo, toConnectTo);
        return instance;
    }

    public Rope CreateRope(RopeConnectionInformation info)
    {
        RopeAnchor anchor = info.Anchor;
        Rope newRope = CreateRope(info.attached, anchor.Rigidbody2D);
        RopeAnchor.RopeSlot slot = anchor.GetEmptySlot();
        anchor.ConnectRopeToSlot(newRope, slot);

        return newRope;
    }

    public Rope AttachPlayerRope(Rigidbody2D rigidbody2D)
    {
        RopeConnectionInformation info = PlayerRopePuller.Instance.DeactivateAndFetchInfo();
        info.attached = rigidbody2D;
        return CreateRope(info);
    }
}
