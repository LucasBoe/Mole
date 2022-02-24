using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryPlayerItem : PlayerItem
{
    [ReadOnly, SerializeField] CarriablePlayerItemWorldObject carriableReference;
    public CarriablePlayerItemWorldObject Carriable
    {
        get => carriableReference;
        set
        {
            carriableReference = value;
        }
    }

    public override bool DisplaySprite ()
    {
        return false;
    }
}
