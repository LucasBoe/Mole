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

    [SerializeField]
    private int[] targetFrameRates = new int[] { -1 };
    private int targetFrameRateIndex = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F4))
        {
            HasGodMode = !HasGodMode;
            DisplayMessage("God Mode: " + HasGodMode);
        }

        if (Input.GetKeyUp(KeyCode.F5))
        {
            targetFrameRateIndex++;

            if (targetFrameRateIndex > targetFrameRates.Length - 1)
                targetFrameRateIndex = 0;

            Application.targetFrameRate = targetFrameRates[targetFrameRateIndex];
            DisplayMessage("Cap Framerate: " + Application.targetFrameRate);
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