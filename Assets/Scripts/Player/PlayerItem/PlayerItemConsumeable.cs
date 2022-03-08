using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerItem/Consumeable", fileName = "Consumeable ")]
public class PlayerItemConsumeable : PlayerItem
{
    public override PlayerItemUseResult UseInteract()
    {
        return new PlayerItemUseResult() { ResultType = PlayerItemUseResult.Type.Destroy, ResultFunction = () => { PlayerHealth.Instance.Heal(int.MaxValue); } };
    }
}
