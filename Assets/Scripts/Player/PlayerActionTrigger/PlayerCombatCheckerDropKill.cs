using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatCheckerDropKill : PlayerBehaviour
{
    [SerializeField] private float updateFrequency = 0.1f;
    ICombatTarget target;
    InputAction dropKillAction;

    void Start()
    {
        StartCoroutine(DoRaycastRoutine());
        dropKillAction = new InputAction() { Input = ControlType.Jump, Stage = InputActionStage.WorldObject, Target = transform, Text = "Drop Kill", ActionCallback = DropKill };
    }

    private void DropKill()
    {
        if (!target.IsNull)
            PlayerStateMachine.Instance.SetState(new DropKillState(target));
        else
            UpdateTarget(null);
    }

    IEnumerator DoRaycastRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateFrequency);
            DoRaycast();
        }
    }

    private void DoRaycast()
    {
        Vector2 start = transform.position + Vector3.down * 3;
        RaycastHit2D[] hits = Physics2D.RaycastAll(start, Vector2.down, 10f, LayerMask.GetMask("Enemy"));

        if (hits.Length > 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                ICombatTarget newTarget = hit.rigidbody.GetComponent<ICombatTarget>();
                if (newTarget != null && newTarget.IsAlive && Util.CheckLineOfSight(transform.position, hit.point, CollisionCheckUtil.GetLineOfSightMask()))
                {
                    Debug.DrawLine(start, hit.point, Color.green, updateFrequency);

                    if (newTarget != target)
                        UpdateTarget(newTarget);
                }
            }
        }
        else
        {
            UpdateTarget(null);
            Debug.DrawRay(start, Vector2.down * 10f, Color.red, updateFrequency);
        }
    }

    private void UpdateTarget(ICombatTarget newTarget)
    {
        target = newTarget;

        if (target != null)
            PlayerInputActionRegister.Instance.RegisterInputAction(dropKillAction);
        else
            PlayerInputActionRegister.Instance.UnregisterAllInputActions(transform);

    }
}
