using PlayerCollisionCheckType;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO: Remake this into player collision handler
public class PlayerController : PlayerSingletonBehaviour<PlayerController>
{
    public static System.Action<Transform> OnPlayerSpawned;

    internal void Teleport(Vector3 teleportPosition)
    {
        transform.position = teleportPosition;
    }

    public static PlayerContext Context => Instance.context;

    [SerializeField] private PlayerContext context;
    [SerializeField] private PlayerValues playerValues;
    [SerializeField] private CheckType[] toDebug;

    private IPlayerComponent[] playerComponents;


    protected override void Awake()
    {
        base.Awake();

        playerComponents = GetComponentsInChildren<IPlayerComponent>();
        playerComponents = playerComponents.OrderBy(c => -c.UpdatePrio).ToArray();

        context = new PlayerContext();
        context.Input = PlayerInputHandler.PlayerInput;
        context.Rigidbody = GetComponent<Rigidbody2D>();
        context.PlayerController = this;
        context.Values = playerValues;

        //base
        context.CollisionChecks.Add(CheckType.Ground, new CollisionCheck(0f, -1.25f, 0.5f, 0.25f, LayerMask.GetMask("Default", "Hangable", "HangableCollidable", "Climbable"), Color.green));
        context.CollisionChecks.Add(CheckType.Hangable, new CollisionCheck(0f, 1.250f, 1.5f, 1.25f, LayerMask.GetMask("Hangable", "HangableCollidable"), Color.yellow));
        context.CollisionChecks.Add(CheckType.HangableJumpInLeft, new CollisionCheck(-0.7f, 0.2f, 0.3f, 1.5f, LayerMask.GetMask("Hangable", "HangableCollidable"), Color.yellow));
        context.CollisionChecks.Add(CheckType.HangableJumpInRight, new CollisionCheck(0.7f, 0.2f, 0.3f, 1.5f, LayerMask.GetMask("Hangable", "HangableCollidable"), Color.yellow));
        context.CollisionChecks.Add(CheckType.ClimbableLeft, new CollisionCheck(-0.55f, 0, 0.45f, 1f, LayerMask.GetMask("Climbable"), Color.green));
        context.CollisionChecks.Add(CheckType.ClimbableRight, new CollisionCheck(0.55f, 0, 0.45f, 1f, LayerMask.GetMask("Climbable"), Color.green));
        context.CollisionChecks.Add(CheckType.WallAbove, new CollisionCheck(0, 1.1f, 1.5f, 0.2f, LayerMask.GetMask("Default"), Color.green));
        context.CollisionChecks.Add(CheckType.Ceiling, new CollisionCheck(0f, 0.875f, 0.75f, 0.25f, LayerMask.GetMask("Default", "HangableCollidable"), Color.gray));
        context.CollisionChecks.Add(CheckType.Body, new CollisionCheck(0f, 0f, 0.875f, 1.5f, LayerMask.GetMask("Default", "Hangable", "HangableCollidable"), Color.gray));

        //details
        context.CollisionChecks.Add(CheckType.HangableLeft, new CollisionCheck(-0.75f, 1.5f, .5f, 1.25f, LayerMask.GetMask("Hangable", "HangableCollidable"), Color.yellow));
        context.CollisionChecks.Add(CheckType.HangableRight, new CollisionCheck(0.75f, 1.5f, .5f, 1.25f, LayerMask.GetMask("Hangable", "HangableCollidable"), Color.yellow));
        context.CollisionChecks.Add(CheckType.HangableAboveAir, new CollisionCheck(0f, 2.875f, 1f, 2f, LayerMask.GetMask("Default", "Hangable", "HangableCollidable"), Color.yellow));
        context.CollisionChecks.Add(CheckType.DropDownable, new CollisionCheck(0, -1.5f, 0.5f, 1f, LayerMask.GetMask("Hangable", "HangableCollidable"), Color.cyan));

        context.CollisionChecks.Add(CheckType.EdgeHelperLeft, new CollisionCheck(-0.4f, -0.75f, 0.4f, 0.75f, LayerMask.GetMask("Default", "Hangable", "HangableCollidable", "Climbable"), Color.cyan));
        context.CollisionChecks.Add(CheckType.EdgeHelperRight, new CollisionCheck(0.4f, -0.75f, 0.4f, 0.75f, LayerMask.GetMask("Default", "Hangable", "HangableCollidable", "Climbable"), Color.cyan));
        context.CollisionChecks.Add(CheckType.AdditionalWallCheck, new CollisionCheck(0, 0, 1.2f, 0.5f, LayerMask.GetMask("Default", "Hangable", "HangableCollidable", "Climbable"), Color.blue));

        //rope
        context.CollisionChecks.Add(CheckType.Rope, new CollisionCheck(0f, 0f, 1.5f, 1.5f, LayerMask.GetMask("Rope"), Color.blue));

        //interactable
        context.CollisionChecks.Add(CheckType.Interactable, new CollisionCheck(0f, -0.5f, 2f, 0.5f, LayerMask.GetMask("Interactable"), Color.blue));

        //tunnel
        context.CollisionChecks.Add(CheckType.Tunnel, new CollisionCheck(0f, 0f, 0.875f, 1.5f, LayerMask.GetMask("TunnelEntrance"), Color.green));

        foreach (IPlayerComponent component in playerComponents)
            component.Init(context);

        OnPlayerSpawned?.Invoke(transform);
    }

    // Update is called once per frame
    void Update()
    {
        context.PlayerPos = transform.position;
        context.IsCollidingToAnyWall = IsColliding(CheckType.ClimbableLeft) || IsColliding(CheckType.ClimbableRight);
        context.TriesMoveLeftRight = context.Input.Axis.x != 0;
        context.TriesMoveUp = context.Input.Axis.y > 0f;
        context.TriesMoveDown = context.Input.Axis.y < 0f;

        foreach (IPlayerComponent component in playerComponents)
            component.UpdatePlayerComponent(context);

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

        foreach (KeyValuePair<CheckType, CollisionCheck> pcc in context.CollisionChecks)
        {
            if (pcc.Value.IsDetecting || toDebug.Contains(pcc.Key))
                Util.GizmoDrawCollisionCheck(pcc.Value, transform.position);
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
        ClimbableLeft,
        ClimbableRight,
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
        AdditionalWallCheck,
        WallAbove,
        Rope,
        Interactable,
        Tunnel,
    }
}
