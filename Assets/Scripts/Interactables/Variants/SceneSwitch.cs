using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : PlayerAboveInputActionProvider
{
    [Scene]
    [SerializeField] string toLoad;
    [SerializeField] SpriteRenderer spriteRenderer;

    protected override InputAction[] CreateInputActions()
    {
        return new InputAction[] { new InputAction()
        {
            ActionCallback = LoadScene,
            Input = ControlType.Interact,
            Target = transform,
            Text = "Step through"
        }};
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(toLoad);
    }
}