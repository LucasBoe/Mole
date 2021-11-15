using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterBasedAnimationBase : ScriptableObject
{
    public virtual void Init(PlayerController playerController)
    {

    }

    public virtual Sprite Update()
    {
        return null;
    }
}
