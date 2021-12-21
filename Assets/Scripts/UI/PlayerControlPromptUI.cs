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

public class PlayerControlPromptUI : TemporaryUIElement
{
    [SerializeField] Image image;
    [SerializeField] Text text;

    public void Init (ControlType type, Vector3 worldPos)
    {
        GetComponent<WorldPositionTrackingUI>().WorldPosition = worldPos;

        switch (type)
        {
            case ControlType.Interact:
                image.color = Color.green;
                text.text = "A";
                break;

            case ControlType.Use:
                image.color = Color.blue;
                text.text = "X";
                break;
        }
    }

    public static PlayerControlPromptUI Show(ControlType type, Vector3 worldPos)
    {
        Debug.LogWarning("Show Prompt");

        PlayerControlPromptUI controlPromptUI = UIHandler.Temporary.Spawn<PlayerControlPromptUI>() as PlayerControlPromptUI;
        controlPromptUI.Init(type, worldPos);

        return controlPromptUI;
    }
}
