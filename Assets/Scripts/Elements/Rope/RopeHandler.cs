using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RopeHandler : SingletonBehaviour<RopeHandler>
{
    [SerializeField] private RopeElement ropePrefab;
    [SerializeField] private RopeEnd ropeEndPrefab;

    private List<Rope> ropes = new List<Rope>();
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

    internal RopeEnd CreateRopeEnd(Vector2 position)
    {
        return Instantiate(ropeEndPrefab, position, Quaternion.identity);
    }

    private void Update()
    {
        foreach (Rope rope in ropes)
            rope.Update();
    }
}
