using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : LightTrigger
{
    EnemyLightModule parent;

    private void OnEnable()
    {
        parent = GetComponentInParent<EnemyLightModule>();
        if (parent != null) parent.Enter(this);
    }

    private void OnDisable()
    {
        if (parent != null) parent.Exit(this);
    }
}
