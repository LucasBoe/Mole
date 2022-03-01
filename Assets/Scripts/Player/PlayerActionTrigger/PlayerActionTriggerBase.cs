using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionTriggerBase : MonoBehaviour
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
    private void Start()
    {
        InputAction = CreateInputAction();
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.IsEnemy() && ConditionsMet(collider))
        {
            ActionTarget = collider.transform;
            PlayerInputActionRegister.Instance.RegisterInputAction(InputAction);
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.IsEnemy())
        {
            PlayerInputActionRegister.Instance.UnregisterInputAction(InputAction);
        }
    }
    protected virtual InputAction CreateInputAction() { return null; }
    protected virtual bool ConditionsMet(Collider2D collider2D) { return false; }
}
