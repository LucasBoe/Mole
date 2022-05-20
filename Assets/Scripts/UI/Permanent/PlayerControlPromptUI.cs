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
            case ControlType.Jump:
                return Color.green;

            case ControlType.Use:
                return Color.blue;

            case ControlType.Interact:
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
            case ControlType.Jump:
                return "A";

            case ControlType.Use:
                return "X";

            case ControlType.Interact:
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
    [SerializeField] Sprite icon_xBox_x, icon_xBox_y, icon_xBox_a, icon_xBox_b; 

    public static System.Action<Transform> OnDeleteControPrompt;
    public void Init (ControlType type, Vector3 worldPos, string txt)
    {
        image.sprite = GetIconFromControlType(type);
        interactionText.text = txt;
    }

    private Sprite GetIconFromControlType(ControlType type)
    {
        switch (type)
        {
            case ControlType.Jump:
                return icon_xBox_y;

            case ControlType.Use:
                return icon_xBox_x;

            case ControlType.Interact:
                return icon_xBox_a;

            case ControlType.Back:
                return icon_xBox_b;
        }

        return null;
    }

    private void OnDestroy()
    {
        OnDeleteControPrompt?.Invoke(transform.parent);
    }
}
