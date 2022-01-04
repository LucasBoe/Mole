using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerInputActionRegister : SingletonBehaviour<PlayerInputActionRegister>
{
    [SerializeField] List<InputAction> actions = new List<InputAction>();

    public void RegisterInputAction(InputAction newAction)
    {

        for (int i = actions.Count - 1; i >= 0; i--)
        {
            InputAction action = actions[i];
            if (action != null && action.Input == newAction.Input)
                actions.Remove(action);
        }

        actions.Add(newAction);
    }

    public void UnregisterInputAction(InputAction oldAction)
    {
        Debug.LogWarning("Unregister " + oldAction);

        for (int i = actions.Count - 1; i >= 0; i--)
        {
            InputAction action = actions[i];
            if (action != null && action.Input == oldAction.Input)
                actions.Remove(action);
        }
    }

    public bool UnregisterInputAction(SpriteRenderer obj)
    {

        Debug.LogWarning("Unregister " + obj);

        for (int i = actions.Count - 1; i >= 0; i--)
        {
            InputAction action = actions[i];
            if (action.Object == obj)
            {
                actions.Remove(action);
                return true;
            }
        }

        return false;
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
