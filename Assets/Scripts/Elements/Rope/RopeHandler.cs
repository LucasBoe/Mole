using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    internal Rope CreateRope(Rigidbody2D start, RopeAnchor[] anchors, Rigidbody2D end)
    {
        Rope newRope = new Rope(start, anchors, end);
        ropes.Add(newRope);

        CheckAndPotentiallyConnectPlayer(newRope, start, end);

        return newRope;
    }

    private void CheckAndPotentiallyConnectPlayer(Rope newRope, Rigidbody2D start, Rigidbody2D end)
    {
        PlayerRopeUser playerStart = start.GetComponentInChildren<PlayerRopeUser>();
        PlayerRopeUser playerEnd = end.GetComponentInChildren<PlayerRopeUser>();

        if (playerStart != null)
            playerStart.ConnectToRope(newRope, playerIsAtStart: true);
        else if (playerEnd != null)
            playerStart.ConnectToRope(newRope, playerIsAtStart: false);
    }

    internal RopeElement CreateRopeElement(Rigidbody2D start, Rigidbody2D end)
    {
        RopeElement instance = Instantiate(ropePrefab, start.position, Quaternion.identity);
        instance.Setup(start, end);
        return instance;
    }

    private void Update()
    {
        foreach (Rope rope in ropes)
            rope.Update();
    }

    private void OnGUI()
    {
        foreach (Rope rope in ropes)
        {
            Handles.Label(rope.Center, rope.Length.ToString());
        }
    }
}
