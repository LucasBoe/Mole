using PlayerCollisionCheckType;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerContext context;

    bool jumpBlocker = false;
    IPlayerComponent[] playerComponents;

    [SerializeField] PlayerValues playerValues;

    private void Awake()
    {
        playerComponents = GetComponentsInChildren<IPlayerComponent>();
        playerComponents = playerComponents.OrderBy(c => -c.UpdatePrio).ToArray();

        context = new PlayerContext();
        context.Input = new PlayerInput();
        context.Rigidbody = GetComponent<Rigidbody2D>();
        context.PlayerController = this;
        context.Values = playerValues;

        //base
        context.CollisionChecks.Add(CheckType.Ground, new CollisionCheck(0f, -1.25f, 0.5f, 0.25f, LayerMask.GetMask("Default", "Hangable", "OneDirectionalFloor", "Pushable"), Color.green));
        context.CollisionChecks.Add(CheckType.Hangable, new CollisionCheck(0f, 1.375f, 1.5f, 1f, LayerMask.GetMask("Hangable"), Color.yellow));
        context.CollisionChecks.Add(CheckType.HangableJumpInLeft, new CollisionCheck(-0.7f, 0.2f, 0.3f, 1.5f, LayerMask.GetMask("Hangable"), Color.yellow));
        context.CollisionChecks.Add(CheckType.HangableJumpInRight, new CollisionCheck(0.7f, 0.2f, 0.3f, 1.5f, LayerMask.GetMask("Hangable"), Color.yellow));
        context.CollisionChecks.Add(CheckType.WallLeft, new CollisionCheck(-0.55f, 0, 0.45f, 1f, LayerMask.GetMask("Default"), Color.green));
        context.CollisionChecks.Add(CheckType.WallRight, new CollisionCheck(0.55f, 0, 0.45f, 1f, LayerMask.GetMask("Default"), Color.green));
        context.CollisionChecks.Add(CheckType.WallAbove, new CollisionCheck(0, 1.1f, 1.5f, 0.2f, LayerMask.GetMask("Default"), Color.green));
        context.CollisionChecks.Add(CheckType.Ceiling, new CollisionCheck(0f, 0.875f, 0.75f, 0.25f, LayerMask.GetMask("Default", "OneDirectionalFloor"), Color.gray));
        context.CollisionChecks.Add(CheckType.Body, new CollisionCheck(0f, 0f, 0.875f, 1.5f, LayerMask.GetMask("Default", "Hangable"), Color.gray));

        //details
        context.CollisionChecks.Add(CheckType.HangableLeft, new CollisionCheck(-0.75f, 1.5f, .5f, 1.25f, LayerMask.GetMask("Hangable"), Color.yellow));
        context.CollisionChecks.Add(CheckType.HangableRight, new CollisionCheck(0.75f, 1.5f, .5f, 1.25f, LayerMask.GetMask("Hangable"), Color.yellow));
        context.CollisionChecks.Add(CheckType.HangableAboveAir, new CollisionCheck(0f, 2.875f, 1f, 2f, LayerMask.GetMask("Default", "Hangable"), Color.yellow));
        context.CollisionChecks.Add(CheckType.DropDownable, new CollisionCheck(0, -1.5f, 0.5f, 1f, LayerMask.GetMask("Hangable", "OneDirectionalFloor"), Color.cyan));
        context.CollisionChecks.Add(CheckType.EdgeHelperLeft, new CollisionCheck(-0.5f, -0.75f, 0.5f, 0.75f, LayerMask.GetMask("Default", "Hangable", "Pushable"), Color.cyan));
        context.CollisionChecks.Add(CheckType.EdgeHelperRight, new CollisionCheck(0.5f, -0.75f, 0.5f, 0.75f, LayerMask.GetMask("Default", "Hangable", "Pushable"), Color.cyan));

        //pushable
        context.CollisionChecks.Add(CheckType.PushableLeft, new CollisionCheck(-0.75f, 0f, 0.5f, 0.75f, LayerMask.GetMask("Pushable"), Color.red));
        context.CollisionChecks.Add(CheckType.PushableRight, new CollisionCheck(0.75f, 0f, 0.5f, 0.75f, LayerMask.GetMask("Pushable"), Color.red));

        foreach (IPlayerComponent component in playerComponents)
            component.Init(context);
    }

    // Update is called once per frame
    void Update()
    {
        //Stick 1 / WASD
        context.Input.Axis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        //Stick 2 / Mouse
        if (new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")).magnitude > 0)
            context.Input.VirtualCursor = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        else
            context.Input.VirtualCursor = ModifyVirtualCursor(context.Input.VirtualCursor, new Vector2(Input.GetAxis("StickRight X"), Input.GetAxis("StickRight Y")));

        context.Input.Back = Input.GetButtonDown("Back");
        context.Input.Jump = Input.GetButtonDown("Jump");
        context.Input.Interact = Input.GetButtonDown("Interact");
        context.Input.Use = Input.GetButtonDown("Use");
        context.Input.Sprint = Input.GetKey(KeyCode.LeftShift);

        context.PlayerPos = transform.position;
        context.IsCollidingToAnyWall = IsColliding(CheckType.WallLeft) || IsColliding(CheckType.WallRight);
        context.TriesMoveLeftRight = context.Input.Axis.x != 0;
        context.TriesMoveUpDown = context.Input.Axis.y != 0f;

        foreach (IPlayerComponent component in playerComponents)
            component.UpdatePlayerComponent(context);

    }

    private Vector2 ModifyVirtualCursor(Vector2 before, Vector2 mouseAxis)
    {
        return (before + mouseAxis * 10).Clamp(Vector2.zero, new Vector2(Screen.width, Screen.height));
    }

    private void FixedUpdate()
    {
        foreach (CollisionCheck pcc in context.CollisionChecks.Values)
            pcc.Update(transform);
    }

    private bool IsColliding(CheckType checkType)
    {
        return context.CollisionChecks[checkType].IsDetecting;
    }

    private void OnDrawGizmos()
    {
        if (context == null || context.CollisionChecks == null)
            return;

        foreach (CollisionCheck pcc in context.CollisionChecks.Values)
        {
            Util.GizmoDrawCollisionCheck(pcc, transform.position);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere((Vector2)transform.position + playerValues.HangableOffset, 0.25f);
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(context.Input.VirtualCursor.x, context.Input.VirtualCursor.y, 10, 10), "");
    }
}

namespace PlayerCollisionCheckType
{
    public enum CheckType
    {
        Ground,
        WallLeft,
        WallRight,
        Ceiling,
        Hangable,
        HangableLeft,
        HangableRight,
        HangableJumpInLeft,
        HangableJumpInRight,
        HangableAboveAir,
        Body,
        DropDownable,
        EdgeHelperLeft,
        EdgeHelperRight,
        PushableLeft,
        PushableRight,
        WallAbove,
    }
}
