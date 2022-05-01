using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class TestHelper : MonoBehaviour
{
    [ShowNonSerializedField] bool showTestMenu;

    // Update is called once per frame
    void Update()
    {
        showTestMenu = Input.GetKey(KeyCode.Tab);
    }

    private void OnGUI()
    {
        if (showTestMenu)
        {
            Vector2 size = new Vector2(100, 300);
            GUILayout.BeginArea(new Rect((Screen.width - size.x) / 2f, (Screen.height - size.y) / 2f, size.x, size.y));
            GUILayout.Button("Test");
            GUILayout.EndArea();
        }
    }
}
