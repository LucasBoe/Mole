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
    public PlayerStateTransitionChecks StateTransitonChecks;

    public ICombatTarget CombatTarget;

}

[System.Serializable]
public class PlayerInput
{

    public Vector2 Axis;
    public Vector2 VirtualCursor;
    public Vector3 VirtualCursorToScreenCenter => (VirtualCursor - new Vector2(Screen.width / 2, Screen.height / 2)) / new Vector2(Screen.width, Screen.height);
    public Vector3 VirtualCursorToWorldPos => CameraController.ScreenToWorldPoint(VirtualCursor);

    public bool ItemMenuUp => MouseWheelUp || DPadUp;
    public bool ItemMenuDown => MouseWheelDown || DPadDown;
    public bool ItemMenuLeft => DPadLeft;
    public bool ItemMenuRight => DPadRight || Tab;


    public float LTAxis;
    public bool LTUp;
    public bool LTDown;
    public Vector2 VirtualCursorToDir(Vector2 position) { return ((Vector2)VirtualCursorToWorldPos - position).normalized; }

    public bool DPadLeft;
    public bool DPadRight;
    public bool DPadUp;
    public bool DPadDown;

    public bool JustPressedOpenInventoryButton;

    public bool Tab;

    public bool MouseWheelUp;
    public bool MouseWheelDown;

    public bool Back;
    public bool Jump;
    public bool Interact;
    public bool Use;

    public bool HoldingBack;
    public bool HoldingJump;
    public bool HoldingInteract;
    public bool HoldingUse;

    public bool Sprinting;

    public bool GetByControlType(ControlType type)
    {
        switch (type)
        {
            case ControlType.Interact:
                return Interact;

            case ControlType.Use:
                return Use;

            case ControlType.Jump:
                return Jump;

            case ControlType.Back:
                return Back;
        }

        return false;
    }

    public void ClearByControlType(ControlType type)
    {
        switch (type)
        {
            case ControlType.Interact:
                Interact = false;
                break;

            case ControlType.Use:
                Use = false;
                break;

            case ControlType.Jump:
                Jump = false;
                break;

            case ControlType.Back:
                Back = false;
                break;
        }

    }

}

public class PlayerStateTransitionChecks
{

    public PlayerStateTransition Rope;
    public PlayerStateTransition Hideable;
    public PlayerStateTransition EnterTunnel;
    public PlayerStateTransition ExitTunnel;
    public PlayerStateTransitionChecks(PlayerContext context)
    {
        Rope = new PlayerStateTransition(context, PlayerState.RopeClimb, CheckType.Rope, ControlType.Use);
        Hideable = new PlayerStateTransition(context, PlayerState.Hiding, CheckType.Hideable, ControlType.Use);
        EnterTunnel = new PlayerStateTransition(context, PlayerState.Tunnel, CheckType.Tunnel, ControlType.Interact);
        ExitTunnel = new PlayerStateTransition(context, PlayerState.Idle, CheckType.Tunnel, ControlType.Interact, needsState: PlayerState.Tunnel);

    }

    internal void TryCheckAll()
    {
        Rope.TryCheck();
        Hideable.TryCheck();

        if (!EnterTunnel.TryCheck())
            ExitTunnel.TryCheck();
    }
}
