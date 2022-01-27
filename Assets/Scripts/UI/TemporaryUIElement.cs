using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryUIElement : MonoBehaviour
{
    public void Hide()
    {
        Debug.Log("Hide UI " + name);
        Destroy(gameObject);
    }
}
