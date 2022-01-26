using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

public class PlayerInputActionRegister : SingletonBehaviour<PlayerInputActionRegister>, IPlayerComponent
{
    [SerializeField] List<InputAction> actions = new List<InputAction>();
    Dictionary<InputAction, PlayerControlPromptUI> actionPromptPair = new Dictionary<InputAction, PlayerControlPromptUI>();

    //needs higher update prio then item user to clear inputs for it
    public int UpdatePrio => 200;

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
        int otherActionsWithSameObject = actions.Where(a => a.Object == action.Object).Count();
        actionPromptPair.Add(action, PlayerControlPromptUI.Show(action.Input, action.Object.transform.position + (Vector3.up * 1.5f) + new Vector3(1f * otherActionsWithSameObject, 0,0), action.Text));
        actions.Add(action);
    }
    private void RemoveAction(InputAction action)
    {
        actionPromptPair[action].Hide();
        actionPromptPair.Remove(action);
        actions.Remove(action);
    }

    public void UpdatePlayerComponent(PlayerContext context)
    {
        for (int i = actions.Count - 1; i >= 0; i--)
        {
            InputAction action = actions[i];
            if (context.Input.GetByControlType(action.Input))
            {
                action.ActionCallback?.Invoke();
                context.Input.ClearByControlType(action.Input);
            }
        }
    }

    void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();

        for (int i = actions.Count - 1; i >= 0; i--)
        {
            InputAction action = actions[i];

            if (action == null || action.Object == null)
            {
                actions.RemoveAt(i);
            }
            else
            {
                style.normal.textColor = action.Input.ToColor();
                Handles.Label(action.Object.transform.position + Vector3.up, action.Input.ToConsoleButtonName() + " -> " + action.Text, style);
            }
        }
    }


    public void Init(PlayerContext context)
    {

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
