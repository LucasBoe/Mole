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
    internal Rope CreateRope(Rigidbody2D player, Rigidbody2D end, Vector2[] travelPoints)
    {
        return CreateRope(player, new RopeAnchor[0], end, travelPoints);
    }

    internal Rope CreateRope(Rigidbody2D start, RopeAnchor[] anchors, Rigidbody2D end, Vector2[] travelPoints = null)
    {
        Rope newRope = new Rope(start, anchors, end, travelPoints);
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

    internal RopeElement CreateRopeElement(Rigidbody2D start, Rigidbody2D end, float length, Vector2[] travelPoints = null)
    {
        RopeElement instance = Instantiate(ropePrefab, start.position, Quaternion.identity, LayerHandler.Parent);
        instance.Setup(start, end, length, travelPoints);
        return instance;
    }

    internal void DestroyRope(Rigidbody2D rigidbody2D)
    {
        Rope toDestroy = null;

        foreach (Rope rope in ropes)
        {
            List<Rigidbody2D> rigidbody2Ds = new List<Rigidbody2D>();
            foreach (RopeElement element in rope.Elements)
            {
                if (element != null)
                {
                    if (element.Rigidbody2DAttachedTo != null) rigidbody2Ds.Add(element.Rigidbody2DAttachedTo);
                    if (element.Rigidbody2DOther != null) rigidbody2Ds.Add(element.Rigidbody2DOther);
                }
            }
            if (rigidbody2Ds.Contains(rigidbody2D))
                toDestroy = rope;
        }

        if (toDestroy != null)
            DestroyRope(toDestroy);
    }

    internal void DestroyRope(Rope rope)
    {
        foreach (RopeElement element in rope.Elements)
        {

            if (element != null)
                element.Destroy();
        }
        ropes.Remove(rope);
    }

    internal RopeEnd CreateRopeEnd(Vector2 position)
    {
        return Instantiate(ropeEndPrefab, position, Quaternion.identity, LayerHandler.Parent);
    }

    private void Update()
    {
        foreach (Rope rope in ropes)
            rope.Update();
    }
}
