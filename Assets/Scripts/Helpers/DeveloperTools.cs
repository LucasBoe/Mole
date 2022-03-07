using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeveloperTools : MonoBehaviour
{
#if UNITY_EDITOR
    public static bool HasGodMode { get; private set; }

    private float lastMessageTime = 0;
    private string lastMessage = "";

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F4))
        {
            HasGodMode = !HasGodMode;
            DisplayMessage("God Mode: " + HasGodMode);
        }
    }

    private void DisplayMessage(string message)
    {
        lastMessageTime = Time.time;
        lastMessage = message;
    }

    private void OnGUI()
    {
        if (lastMessageTime + 5 > Time.time && lastMessage != "")
            GUI.Box(new Rect(Screen.width - 200, 20, 200, 20), lastMessage);
    }
#endif
}