using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerItem/Consumeable", fileName = "Consumeable ")]
public class PlayerItemConsumeable : PlayerItem
{
    public override PlayerItemUseResult AimInteract(PlayerItemUser playerItemUser, int selectedModeIndex)
    {
        return new PlayerItemUseResult() { ResultType = PlayerItemUseResult.Type.Destroy, ResultFunction = () => { PlayerHealth.Instance.Heal(int.MaxValue); } };
    }
}
