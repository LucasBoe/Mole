using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullifyRotation : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;
    }
}
