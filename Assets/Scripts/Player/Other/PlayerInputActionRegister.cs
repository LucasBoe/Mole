using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class PlayerInputActionRegister : SingletonBehaviour<PlayerInputActionRegister>
{
    [SerializeField] List<InputAction> actions = new List<InputAction>();
    Dictionary<InputAction, PlayerControlPromptUI> actionPromptPair = new Dictionary<InputAction, PlayerControlPromptUI>();

    public void RegisterInputAction(InputAction newAction)
    {
        for (int i = actions.Count - 1; i >= 0; i--)
        {
            InputAction action = actions[i];
            if (action != null && action.Input == newAction.Input)
                RemoveAction(action);
        }

        AddAction(newAction);
    }


    public void UnregisterInputAction(InputAction oldAction)
    {
        for (int i = actions.Count - 1; i >= 0; i--)
        {
            InputAction action = actions[i];
            if (action != null && action.Input == oldAction.Input)
                RemoveAction(action);
        }
    }

    public bool UnregisterInputAction(SpriteRenderer obj)
    {
        for (int i = actions.Count - 1; i >= 0; i--)
        {
            InputAction action = actions[i];
            if (action.Object == obj)
            {
                RemoveAction(action);
                return true;
            }
        }

        return false;
    }
    private void AddAction(InputAction action)
    {
        actionPromptPair.Add(action, PlayerControlPromptUI.Show(action.Input, action.Object.transform.position));
        actions.Add(action);
    }
    private void RemoveAction(InputAction action)
    {
        actionPromptPair[action].Hide();
        actionPromptPair.Remove(action);
        actions.Remove(action);
    }

    private void Update()
    {
        for (int i = actions.Count - 1; i >= 0; i--)
        {
            InputAction action = actions[i];
            if (PlayerInputHandler.PlayerInput.GetByControlType(action.Input))
                action.ActionCallback?.Invoke();
        }
    }
    void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();

        foreach (InputAction action in actions)
        {
            style.normal.textColor = action.Input.ToColor();
            Handles.Label(action.Object.transform.position + Vector3.up, action.Input.ToConsoleButtonName() + " -> " + action.Text, style);
        }
    }
}

[System.Serializable]
public class InputAction
{
    public ControlType Input;
    public SpriteRenderer Object;
    public string Text = "Interact";
    public System.Action ActionCallback;
}

public interface IInputActionProvider
{
    InputAction FetchInputAction();
}
