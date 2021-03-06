using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : PlayerSingletonBehaviour<PlayerInputHandler>
{
    public static PlayerInput PlayerInput = new PlayerInput();
    [SerializeField] PlayerInput debug;


    bool DPadLock = false;

    public static System.Action<PlayerInput> OnPlayerInput;

    private void Update()
    {
        //Stick 1 / WASD
        PlayerInput.Axis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        PlayerInput.AxisRight = new Vector2(Input.GetAxis("StickRight X"), Input.GetAxis("StickRight Y"));

        //Stick 2 / Mouse => virtual cursor
        if (new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")).magnitude > 0)
            PlayerInput.VirtualCursor = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        else
            PlayerInput.VirtualCursor = ModifyVirtualCursor(PlayerInput.VirtualCursor, PlayerInput.AxisRight);

        float dPadX = Input.GetAxis("Cross X");
        float dPadY = Input.GetAxis("Cross Y");

        PlayerInput.DPadUp = dPadY > 0.1f && !DPadLock;
        PlayerInput.DPadDown = dPadY < -0.1f && !DPadLock;
        PlayerInput.DPadLeft = dPadX < -0.1f && !DPadLock;
        PlayerInput.DPadRight = dPadX < -0.1f && !DPadLock;

        PlayerInput.Tab = Input.GetKeyDown(KeyCode.Tab);

        if (PlayerInput.DPadUp || PlayerInput.DPadDown || PlayerInput.DPadLeft || PlayerInput.DPadRight) DPadLock = true;
        if (DPadLock && dPadY == 0 && dPadX == 0) DPadLock = false;

        PlayerInput.MouseWheelDown = Input.GetAxis("Mouse ScrollWheel") < 0f;
        PlayerInput.MouseWheelUp = Input.GetAxis("Mouse ScrollWheel") > 0f;

        PlayerInput.JustPressedOpenInventoryButton = Input.GetKeyDown(KeyCode.I) || Input.GetAxis("Cross Y") != 0;

        PlayerInput.LTAxis = Input.GetAxis("LT");
        PlayerInput.LTDown = !PlayerInput.LT && PlayerInput.LTAxis > 0.5f;
        PlayerInput.LTUp = PlayerInput.LT && PlayerInput.LTAxis < 0.5f;
        PlayerInput.LT = PlayerInput.LTAxis > 0.5f;

        PlayerInput.Back = Input.GetButtonDown("Back");
        PlayerInput.Jump = Input.GetButtonDown("Jump");
        PlayerInput.Interact = Input.GetButtonDown("Interact");
        PlayerInput.Use = Input.GetButtonDown("Use");

        PlayerInput.HoldingBack = Input.GetButton("Back");
        PlayerInput.HoldingJump = Input.GetButton("Jump");
        PlayerInput.HoldingInteract = Input.GetButton("Interact");
        PlayerInput.HoldingUse = Input.GetButton("Use");
        PlayerInput.Sprinting = Input.GetAxis("Sprint") > 0.5f || Input.GetButton("Sprint");

        debug = PlayerInput;
        OnPlayerInput?.Invoke(PlayerInput);
    }

    private Vector2 ModifyVirtualCursor(Vector2 before, Vector2 mouseAxis)
    {
        return (before + mouseAxis.InvertY() * 10).Clamp(Vector2.zero, new Vector2(Screen.width, Screen.height));
    }
}
