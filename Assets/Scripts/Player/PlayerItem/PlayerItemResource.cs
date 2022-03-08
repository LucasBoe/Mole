using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerItem/Resource", fileName = "Resource")]
public class PlayerItemResource : PlayerItem
{
    public override bool IsUseable => false;
}
