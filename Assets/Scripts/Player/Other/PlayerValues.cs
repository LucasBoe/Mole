using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerValues : ScriptableObject
{
    public SprintEffectedFloat XVelocity = new SprintEffectedFloat { SprintValue = 9, NotSprintValue = 5 };
    public SprintEffectedFloat StraveXVelocity = new SprintEffectedFloat { SprintValue = 10, NotSprintValue = 6 };
    public float WallClimbYvelocity = 6f;
    public float EdgeHelperUpwardsImpulse = 1f;

    public SprintEffectedFloat JumpForce = new SprintEffectedFloat { SprintValue = 50, NotSprintValue = 30 };
    [Range(0,1)]
    public float DirectionalJumpAmount = 0.5f;
    public AnimationCurve AdditionalGravityForce = AnimationCurve.Linear(0,1,2,4);

    public float PullUpDuration = 0.5f;

    public AnimationCurve PullUpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public float DropDownSpeed = 25f;


    public Vector2 HangableOffset = new Vector2(0, 1.25f);

    //pushes the player to the wall while in wall state
    public float WallPushVelocity = 0.1f;
    public float WallSnapXVelocity = 4f;
    public float JumpOffVelocity = 1.5f;
    public float KeyPressTimeToDropDown = 0.5f;

    public float SnapToHideablePositionDuration = 0.5f;
}

[System.Serializable]
public struct SprintEffectedFloat
{
    public float SprintValue;
    public float NotSprintValue;

    public float GetValue(PlayerInput input)
    {
        return input.Sprinting ? SprintValue : NotSprintValue;
    }
}
