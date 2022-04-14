using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSwitchAndTeleport : LayerSwitch
{
    [SerializeField] Vector3 localTeleportTarget;

    protected override void SwitchLayer()
    {
        base.SwitchLayer();
        PlayerController.Instance.Teleport(ToGlobal(localTeleportTarget));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(ToGlobal(localTeleportTarget), 0.5f);
    }

    private Vector3 ToGlobal(Vector3 local)
    {
        return transform.TransformPoint(local);
    }
}
