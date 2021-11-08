using System.Collections;
using System.Collections.Generic;
using PlayerCollisionCheckType;
using UnityEngine;

public enum PlayerMovementState
{
    Idle,
    Walk,
    Run,
    Climb,
}

public enum PlayerClimbState
{
    None,
    Wall,
    Ceiling,
    Hanging,
    Transition,
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerClimbState climbState;
    [SerializeField] PlayerMovementState moveState;

    [SerializeField] bool jumpBlocker = false;

    Dictionary<CheckType, PlayerCollisionCheck> collisionChecks = new Dictionary<CheckType, PlayerCollisionCheck>();

    Rigidbody2D rigidbody;

    [SerializeField] private float jumpForce, walkForce, wallClimbForce, additionalGravityForce;

    public System.Action<PlayerMovementState, PlayerClimbState> OnStateChange;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        collisionChecks.Add(CheckType.Ground, new PlayerCollisionCheck(0f, -1.125f, 0.75f, 0.25f, LayerMask.GetMask("Default")));
        collisionChecks.Add(CheckType.Hangable, new PlayerCollisionCheck(0f, 1.5f, 1.5f, 1f, LayerMask.GetMask("Hangable")));
        collisionChecks.Add(CheckType.WallLeft, new PlayerCollisionCheck(-0.75f, 0.5f, 0.5f, 1f, LayerMask.GetMask("Default")));
        collisionChecks.Add(CheckType.WallRight, new PlayerCollisionCheck(0.75f, 0.5f, 0.5f, 1f, LayerMask.GetMask("Default")));
        collisionChecks.Add(CheckType.Ceiling, new PlayerCollisionCheck(0f, 1f, 0.75f, 0.25f, LayerMask.GetMask("Default")));
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (PlayerCollisionCheck pcc in collisionChecks.Values)
        {
            pcc.Update(transform);
        }

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        switch (moveState)
        {
            case PlayerMovementState.Climb:

                if (climbState == PlayerClimbState.Wall)
                {
                    if (!IsColliding(CheckType.WallLeft) && !IsColliding(CheckType.WallRight) || IsColliding(CheckType.Ground))
                    {
                        SetState(PlayerMovementState.Walk);
                    } else
                    {
                        rigidbody.AddForce(Vector2.up * input.y * Time.deltaTime * 1000f * wallClimbForce, ForceMode2D.Impulse);
                    }
                }

                break;

            default:

                if (input.x != 0)
                    rigidbody.AddForce(Vector2.right * input.x * 1000f * walkForce * Time.deltaTime);

                if (IsColliding(CheckType.Ground))
                {
                    if (!jumpBlocker && Input.GetButton("Jump"))
                    {
                        jumpBlocker = true;
                        rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                        Invoke("ResetJumpBlocker", 0.5f);
                    }
                }
                else
                {
                    rigidbody.AddForce(new Vector2(0, -Time.deltaTime * 1000f * additionalGravityForce));

                    if ((IsColliding(CheckType.WallLeft) || IsColliding(CheckType.WallRight)) && input.y != 0f)
                    {
                        SetState(PlayerClimbState.Wall);
                    }    
                }
                break;
        }
    }

    public void SetState(PlayerClimbState climbState)
    {
        SetState(PlayerMovementState.Climb, climbState);
    }

    public void SetState(PlayerMovementState moveState, PlayerClimbState climbState = PlayerClimbState.None)
    {
        rigidbody.gravityScale = moveState == PlayerMovementState.Climb ? 0 : 2;

        switch (moveState)
        {
            case PlayerMovementState.Climb:

                switch(climbState)
                {
                    default:
                        //
                        break;
                }
                break;
        }

        this.moveState = moveState;
        this.climbState = climbState;
        OnStateChange?.Invoke(this.moveState, this.climbState);
    }

    private bool IsColliding(CheckType checkType)
    {
        return collisionChecks[checkType].IsDetecting;
    }

    private void OnDrawGizmos()
    {
        foreach (PlayerCollisionCheck pcc in collisionChecks.Values)
        {
            Gizmos.color = pcc.IsDetecting ? Color.yellow : Color.white;
            Gizmos.DrawWireCube((Vector2)transform.position + pcc.Pos, pcc.Size);
        }
    }

    private void OnGUI()
    {
        foreach (KeyValuePair<CheckType, PlayerCollisionCheck> pcc in collisionChecks)
        {
            Vector3 offsetFromSize = new Vector3(0.1f + ((Vector3)(pcc.Value.Size / 2f)).x, ((Vector3)(pcc.Value.Size / 2f)).y);
            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position + (Vector3)pcc.Value.Pos + offsetFromSize);
            Rect rect = new Rect(screenPos.x, Screen.height - screenPos.y, 150, 50);
            GUI.Label(rect, pcc.Key.ToString() + " (" + pcc.Value.LayerMask.ToString() + ")");
        }
    }

    private void ResetJumpBlocker()
    {
        jumpBlocker = false;
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
    }
}
