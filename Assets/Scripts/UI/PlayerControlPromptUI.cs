using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ControlType
{
    Interact,
    Use,
    Jump,
    Back,
}

public static class ControlTypeExtention
{
    public static Color ToColor(this ControlType type)
    {
        switch (type)
        {
            case ControlType.Interact:
                return Color.green;

            case ControlType.Use:
                return Color.blue;

            case ControlType.Jump:
                return Color.yellow;

            case ControlType.Back:
                return Color.red;
        }

        return Color.white;
    }
    public static string ToConsoleButtonName(this ControlType type)
    {
        switch (type)
        {
            case ControlType.Interact:
                return "A";

            case ControlType.Use:
                return "X";

            case ControlType.Jump:
                return "Y";

            case ControlType.Back:
                return "B";
        }

        return "";
    }
}

public class PlayerControlPromptUI : TemporaryUIElement
{
    [SerializeField] Image image;
    [SerializeField] TMP_Text buttonText, interactionText;

    public static System.Action<Transform> OnDeleteControPrompt;
    public void Init (ControlType type, Vector3 worldPos, string txt)
    {
        image.color = type.ToColor();
        buttonText.text = type.ToConsoleButtonName();
        interactionText.text = txt;
    }

    private void OnDestroy()
    {
        OnDeleteControPrompt?.Invoke(transform.parent);
    }
}
