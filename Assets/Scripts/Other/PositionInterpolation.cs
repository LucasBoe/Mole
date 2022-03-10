using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionInterpolation
{
    private float startTime;
    private Vector2 start, end;
    private AnimationCurve curve;
    private float duration;
    public bool Done = false;

    public PositionInterpolation(Vector2 start, Vector2 end, AnimationCurve curve, float duration = -1f, float speed = 1f)
    {
        this.startTime = Time.time;
        this.start = start;
        this.end = end;
        this.curve = curve;

        if (duration != -1)
            this.duration = duration;
        else
            this.duration = Vector2.Distance(start, end) / speed;
    }

    public Vector2 Evaluate()
    {
        if (Done)
            return end;

        float time = Time.time - startTime;
        Done = time >= duration;

        float lerp = curve.Evaluate(time / duration);
        return Vector2.Lerp(start, end, lerp);
    }
}
