using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ClothlineElement : BasicHangable, ISlideable
{

    public Vector2 GetClosestPosition(Vector2 playerPos, Vector2 input)
    {

        return GetClosestHangablePosition(playerPos, input);
    }
}
