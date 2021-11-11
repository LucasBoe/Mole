using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerValues : ScriptableObject
{
    internal float walkXvelocity = 8f;
    public float StraveXVelocity = 6f;

    public float JumpForce = 30f;

    public float PullUpDuration = 0.5f;

    public AnimationCurve PullUpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public float DropDownSpeed = 25f;

    public float WallClimbYvelocity = 6f;

    public Vector2 HangableOffset = new Vector2(0, 1.25f);
}
