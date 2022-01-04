using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerValues : ScriptableObject
{
    public float WalkXvelocity = 9f;
    public float CrouchXvelocity = 5f;
    public float TunnelXvelocity = 3f;
    public float StraveXVelocity = 6f;
    public float WallClimbYvelocity = 6f;
    public float EdgeHelperUpwardsImpulse = 1f;

    public float JumpForce = 30f;
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
