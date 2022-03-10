#if UNITY_EDITOR 

using UnityEditor;
using UnityEngine;
public class LayerFixer : MonoBehaviour
{
    // Add a menu item named "Do Something" to MyMenu in the menu bar.
    [MenuItem("LayerSystem/Fix Layers For Camera System")]
    static void DoSomething()
    {
        string message = "LayerFixer: changed layers for layers to...\n";

        foreach (GameObject layer in LayerHandler.EditorInstance.LayerGameObjects)
        {
            message += "" + layer.name + "\n";

            foreach (Transform child in layer.transform)
            {
                if (child.name == "")
                {
                    break;
                }
                else if (child.name == "Foreground")
                {
                    child.gameObject.layer = LayerMask.NameToLayer("Default");
                } else
                {
                    child.gameObject.layer = LayerMask.NameToLayer("Background");
                }

                message += " - " + child.gameObject.name + " => " +  LayerMask.LayerToName(child.gameObject.layer) + "\n";
            }

        }
        Debug.Log(message);
    }
}
#endif