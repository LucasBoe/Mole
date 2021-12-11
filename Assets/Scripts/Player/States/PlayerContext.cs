using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;
using System;

public class PlayerContext
{
    public Dictionary<CheckType, CollisionCheck> CollisionChecks = new Dictionary<CheckType, CollisionCheck>();
    public PlayerController PlayerController;
    public PlayerValues Values;
    public Rigidbody2D Rigidbody;
    public Vector2 PlayerPos;
    public bool IsCollidingToAnyWall;
    internal bool TriesMoveLeftRight;
    internal bool TriesMoveUpDown;
    public PlayerInput Input;
}

public class PlayerInput
{
    public Vector2 Axis;
    public Vector2 VirtualCursor;
    public bool Back;
    public bool Jump;
    public bool Interact;
    public bool Use;
    public bool Sprint;

    public Vector3 VirtualCursorToScreenCenter => (VirtualCursor - new Vector2(Screen.width / 2, Screen.height / 2)) / new Vector2(Screen.width, Screen.height).InvertY();

    internal Vector2 VirtualCursorToDir(Vector2 position)
    {
        Vector2 cursorToWorld = CameraController.ScreenToWorldPoint(VirtualCursor);
        Debug.DrawLine(position, cursorToWorld);

        return (cursorToWorld - position).normalized;
    }
}
