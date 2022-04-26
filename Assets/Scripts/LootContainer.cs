using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootContainer
{
    [SerializeField] private PlayerItem[] loot;
    public bool CanLoot => loot != null && loot.Length > 0;

    private PlayerItem[] Loot ()
    {
        if (!CanLoot) return null;

        PlayerItem[] toReturn = loot;
        loot = null;
        return toReturn;
    }

    public bool PlayerTryLoot()
    {
        Debug.Log("PlayerTryLoot");

        if (!CanLoot) return false;

        foreach (PlayerItem item in Loot())
        {
            WorldTextSpawner.Spawn(item.Sprite ,"+1 " + item.name, PlayerItemHolder.Instance.transform.position + Vector3.up);
            PlayerItemHolder.Instance.AddItem(item);
        }

        return true;
    }

    internal string GetName()
    {
        if (!CanLoot) return "<EMPTY>";
        return loot[0].name + (loot.Length > 1 ? ", ..." : "");
    }
}
