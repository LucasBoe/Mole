using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerItem : ScriptableObject
{
    public Sprite Sprite;
    public GameObject Prefab;
    public bool NeedsConfirmation;
    public bool IsHeavy;
    public virtual bool IsUseable => true;

    public virtual PlayerItemUseResult UseInteract() { return new PlayerItemUseResult(); }
    public virtual PlayerItemUseResult ConfirmInteract(PlayerItemUser playerItemUser, int selectedModeIndex) { return new PlayerItemUseResult(); }

    public virtual void AimUpdate(PlayerItemUser playerItemUser, PlayerContext context, LineRenderer aimLine) { }

    public virtual ItemMode[] GetItemModes()
    {
        return new ItemMode[] { new ItemMode() { Icon = Sprite, Name = "Item" } };
    }

    public virtual bool DisplaySprite()
    {
        return true;
    }

    public virtual bool IsCarryable()
    {
        return false;
    }
}

public class PlayerItemUseResult
{
    public Type ResultType;
    public Action ResultFunction;
    public float ResultFloat;
    public string ResultString;

    public PlayerItemUseResult()
    {
        ResultType = Type.Fail;
    }

    public PlayerItemUseResult(Type type)
    {
        ResultType = type;
    }

    public PlayerItemUseResult(Action resultFunction)
    {
        ResultType = Type.Function;
        ResultFunction = resultFunction;
    }

    public PlayerItemUseResult(Type type, float resultFloat)
    {
        ResultType = type;
        ResultFloat = resultFloat;
    }

    public PlayerItemUseResult(Type type, string resultString)
    {
        ResultType = type;
        ResultString = resultString;
    }

    public enum Type
    {
        None,
        Fail,
        Destroy,
        Function,
        StartAim,
        InCooldown,
    }
}
