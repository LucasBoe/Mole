using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISlideable
{
    Vector2 GetClosestPosition(Vector2 playerPos, Vector2 axis);
}
