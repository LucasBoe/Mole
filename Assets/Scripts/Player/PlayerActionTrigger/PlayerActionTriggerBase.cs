using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class PlayerActionTriggerBase : PlayerBehaviour
{
    private InputAction inputAction;
    protected InputAction InputAction { get => inputAction; set { inputAction = value; } }

    private Transform actionTarget;
    protected Transform ActionTarget
    {
        get => actionTarget;
        set
        {
            actionTarget = value;
            inputAction.Target = value;
        }
    }
    private Collider2D trigger;

    private void Awake()
    {
        trigger = GetComponent<Collider2D>();
    }

    protected virtual void Start()
    {
        InputAction = CreateInputAction();
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.IsEnemy())
        {
            OnCombatTargetEnter(collider.GetComponent<ICombatTarget>());

            if (ConditionsMet(collider))
            {
                ActionTarget = collider.transform;
                PlayerInputActionRegister.Instance.RegisterInputAction(InputAction);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.IsEnemy())
        {
            OnCombatTargetExit(collider.GetComponent<ICombatTarget>());
            PlayerInputActionRegister.Instance.UnregisterInputAction(InputAction);
        }
    }
    protected virtual void OnCombatTargetEnter(ICombatTarget target) { }

    protected virtual void OnCombatTargetExit(ICombatTarget target) { }

    protected void UpdateTrigger()
    {
        Vector2 offset = trigger.offset;
        trigger.enabled = false;
        trigger.offset = Vector2.down;
        trigger.enabled = true;
        trigger.offset = offset;
    }
    protected abstract InputAction CreateInputAction();
    protected abstract bool ConditionsMet(Collider2D collider2D);
}
