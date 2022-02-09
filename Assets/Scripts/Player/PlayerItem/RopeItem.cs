using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RopeItem : ThrowablePlayerItem
{
    [SerializeField] ItemMode ropeThrow;
    [SerializeField] RopeHook hookPrefab;
    public override ItemMode[] GetItemModes()
    {
        return new ItemMode[] { ropeThrow };
    }

    public override GameObject GetObjectToInstatiate()
    {
        return hookPrefab.gameObject;
    }
}
