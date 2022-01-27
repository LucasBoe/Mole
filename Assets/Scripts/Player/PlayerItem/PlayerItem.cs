using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItem : ScriptableObject
{
    public Sprite Sprite;
    public CollectablePlayerItem Prefab;
    public bool IsUseable;
    public bool HandOnly = false;

    public virtual void AimUpdate(PlayerItemUser playerItemUser, PlayerContext context, LineRenderer aimLine) { }

    public virtual PlayerItemUseResult AimInteract(PlayerItemUser playerItemUser)
    {
        return new PlayerItemUseResult();
    }

    public virtual ItemMode[] GetItemModes()
    {
        return new ItemMode[] { new ItemMode() { Icon = Sprite, Name = "Item" } };
    }
}

public class PlayerItemUseResult
{
    public Type ResultType;
    public Action ResultFunction;

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

    public enum Type
    {
        None,
        Fail,
        Destroy,
        Function,
    }
}
