using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TemporaryUIElement : MonoBehaviour
{
    public void Hide()
    {
        Destroy(gameObject);
    }
}
