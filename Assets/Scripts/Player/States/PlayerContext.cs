using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;

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

}
