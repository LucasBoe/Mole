using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ControlType
{
    Interact,
}

public class PlayerControlPromptUI : SingletonBehaviour<PlayerControlPromptUI>
{
    [SerializeField] Image image;
    [SerializeField] Text text;

    public void Hide ()
    {
        image.enabled = false;
        text.enabled = false;
    }

    public void Show (ControlType type, Vector3 worldPos)
    {
        transform.position = CameraController.WorldToScreenPoint(worldPos);

        switch (type)
        {
            case ControlType.Interact:
                image.color = Color.green;
                text.text = "A";
                break;
        }

        image.enabled = true;
        text.enabled = true;
    }
}
