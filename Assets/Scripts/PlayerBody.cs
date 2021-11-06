using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    Vector2 localPositionTarget;
    float rotationTarget;

    public void UpdateBody(PlayerCollisionCheck footCheck, PlayerCollisionCheck leftCheck, PlayerCollisionCheck rightCheck, PlayerCollisionCheck ceilingCheck)
    {
        rotationTarget = 0;

        if (footCheck.IsDetecting)
        {
            localPositionTarget = new Vector2(0, 0.5f);
        }
        else
        {
            localPositionTarget = new Vector2(0, 0f);

            if (ceilingCheck.IsDetecting)
            {

                if (leftCheck.IsDetecting)
                {
                    rotationTarget = -45f;
                }
                else if (rightCheck.IsDetecting)
                {
                    rotationTarget = 45;
                }
                else
                {
                    rotationTarget = 90f;
                }
            }
        }


        transform.localPosition = Vector3.MoveTowards(transform.localPosition, localPositionTarget, Time.deltaTime); 
        transform.localRotation = Quaternion.Euler(0, 0, rotationTarget);
    }
}
