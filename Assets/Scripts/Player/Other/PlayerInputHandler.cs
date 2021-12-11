using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : SingletonBehaviour<PlayerInputHandler>
{
    public static PlayerInput PlayerInput = new PlayerInput();

    private void Update()
    {
        //Stick 1 / WASD
        PlayerInput.Axis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        //Stick 2 / Mouse
        if (new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")).magnitude > 0)
            PlayerInput.VirtualCursor = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        else
            PlayerInput.VirtualCursor = ModifyVirtualCursor(PlayerInput.VirtualCursor, new Vector2(Input.GetAxis("StickRight X"), Input.GetAxis("StickRight Y")));

        PlayerInput.Back = Input.GetButtonDown("Back");
        PlayerInput.Jump = Input.GetButtonDown("Jump");
        PlayerInput.Interact = Input.GetButtonDown("Interact");
        PlayerInput.Use = Input.GetButtonDown("Use");
        PlayerInput.Sprint = Input.GetKey(KeyCode.LeftShift);
    }

    private Vector2 ModifyVirtualCursor(Vector2 before, Vector2 mouseAxis)
    {
        return (before + mouseAxis * 10).Clamp(Vector2.zero, new Vector2(Screen.width, Screen.height));
    }
}
