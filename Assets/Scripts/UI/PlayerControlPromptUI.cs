using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ControlType
{
    Interact,
    Use,
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
        }

        return "";
    }
}

public class PlayerControlPromptUI : TemporaryUIElement
{
    [SerializeField] Image image;
    [SerializeField] Text text;

    public void Init (ControlType type, Vector3 worldPos)
    {
        GetComponent<WorldPositionTrackingUI>().WorldPosition = worldPos;
        image.color = type.ToColor();
        text.text = type.ToConsoleButtonName();
    }

    public static PlayerControlPromptUI Show(ControlType type, Vector3 worldPos)
    {
        PlayerControlPromptUI controlPromptUI = UIHandler.Temporary.Spawn<PlayerControlPromptUI>() as PlayerControlPromptUI;
        controlPromptUI.Init(type, worldPos);

        return controlPromptUI;
    }
}
