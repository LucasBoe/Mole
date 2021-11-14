using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;

public class PlayerContext
{
    public Dictionary<CheckType, CollisionCheck> CollisionChecks = new Dictionary<CheckType, CollisionCheck>();
    internal Vector2 Input;
    public PlayerController PlayerController;
    public PlayerValues Values;
    public Rigidbody2D Rigidbody;
    public Vector2 PlayerPos;
    public bool IsCollidingToAnyWall;
    internal bool TriesMoveLeftRight;
    internal bool TriesMoveUpDown;
    public bool IsJumping;

}
