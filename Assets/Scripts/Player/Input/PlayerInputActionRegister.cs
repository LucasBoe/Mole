using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

public enum InputActionStage
{
    ModeSpecific,
    WorldObject,
}

public class PlayerInputActionRegister : PlayerSingletonBehaviour<PlayerInputActionRegister>, IPlayerComponent
{
    [SerializeField] Dictionary<ControlType, List<InputAction>> register = new Dictionary<ControlType, List<InputAction>>();
    public static Dictionary<ControlType, List<InputAction>> Register => Instance.register;
    public static System.Action<ControlType> OnInputActionChangedForType;

    public static InputAction GetTopActionForType(ControlType type)
    {
        if (Register.ContainsKey(type) && Register[type].Count > 0)
            return Register[type].OrderBy(ac => ac.Stage).First();
        else
            return null;
    }
    //needs higher update prio then item user to clear inputs for it
    public int UpdatePrio => 200;
    public bool debug;

    public void Init(PlayerContext context)
    {
        register.Add(ControlType.Interact, new List<InputAction>());
        register.Add(ControlType.Use, new List<InputAction>());
        register.Add(ControlType.Jump, new List<InputAction>());
        register.Add(ControlType.Back, new List<InputAction>());
    }

    public void RegisterInputAction(InputAction newAction)
    {
        ControlType controlType = newAction.Input;
        if (!register[controlType].Contains(newAction))
        {
            register[controlType].Add(newAction);
            OnInputActionChangedForType?.Invoke(controlType);
        }
    }

    public void UnregisterInputAction(InputAction oldAction)
    {
        if (oldAction == null)
            return;


        ControlType controlType = oldAction.Input;
        if (register[controlType].Contains(oldAction))
        {
            register[controlType].Remove(oldAction);
            OnInputActionChangedForType?.Invoke(controlType);
        }
    }

    public bool UnregisterAllInputActions(UnityEngine.Object target)
    {
        List<ControlType> typesToUpdate = new List<ControlType>();

        bool removed = false;
        foreach (ControlType type in register.Keys)
        {
            var list = register[type];
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].TargetObject == target)
                {
                    if (!typesToUpdate.Contains(type))
                        typesToUpdate.Add(type);

                    list.RemoveAt(i);
                    removed = true;
                }
            }
        }

        foreach (ControlType type in typesToUpdate)
        {
            OnInputActionChangedForType?.Invoke(type);
        }

        return removed;
    }

    public void UpdatePlayerComponent(PlayerContext context)
    {
        for (int i = 0; i < 4; i++)
        {
            ControlType type = ((ControlType)i);
            if (register.ContainsKey(type) && context.Input.GetByControlType(type) && register[type].Count > 0)
            {
                GetTopActionForType(type).ActionCallback?.Invoke();
            }
        }
    }

    private void OnGUI()
    {
        if (debug)
        {
            for (int i = 0; i < 4; i++)
            {
                if (register.ContainsKey((ControlType)i))
                {
                    string str = ((ControlType)i).ToString() + ":\n";

                    foreach (InputAction action in register[(ControlType)i].OrderBy(ac => ac.Stage))
                    {
                        if (action.TargetObject != null)
                            str += action.Text + " (" + action.TargetObject.name + " - " + action.Stage.ToString() + " )\n";
                    }

                    GUI.Box(new Rect(100 + 200 * i, 100, 190, 90), str);
                }
            }
        }
    }
}

[System.Serializable]
public class InputAction
{
    public ControlType Input;
    public string Text = "Interact";
    public System.Action ActionCallback;
    public InputActionStage Stage;

    public TargetTypes TargetType { get; private set; }
    public Transform TargetTransform { get; private set; }
    public RectTransform TargetRectTransform { get; private set; }
    public UnityEngine.Object TargetObject { get; private set; }

    public UnityEngine.Object Target { set => SetTarget(value); }
    public void SetTarget(UnityEngine.Object target)
    {
        if (target.GetType() == typeof(RectTransform))
        {
            TargetType = TargetTypes.RectTransform;
            TargetRectTransform = target as RectTransform;
            TargetTransform = target as Transform;
        }
        else if (target.GetType() == typeof(Transform))
        {
            TargetType = TargetTypes.Transform;
            TargetTransform = target as Transform;
        }
        else
        {
            TargetType = TargetTypes.Object;
        }
        TargetObject = target;
    }

    public enum TargetTypes
    {
        Transform,
        RectTransform,
        Object,
    }
}

public interface IInputActionProvider
{
    InputAction FetchInputAction();
}
