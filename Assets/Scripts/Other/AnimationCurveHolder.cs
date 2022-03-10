using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCurveHolder : SingletonBehaviour<AnimationCurveHolder>
{
    [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0,0,1,1);
    public static AnimationCurve Ease => Instance.ease;
}
