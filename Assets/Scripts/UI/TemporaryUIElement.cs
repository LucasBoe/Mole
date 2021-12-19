using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryUIElement : MonoBehaviour
{
    public void Hide()
    {
        Destroy(gameObject);
    }
}
