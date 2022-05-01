using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerShakerTester : MonoBehaviour
{
    [SerializeField, Range(0, 10)] float amplitude = 1;
    [SerializeField, Range(0, 1)] float frequency = 0.1f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            CameraShaker.Instance.Shake(PlayerUtil.Position, strength: amplitude, frequency: frequency);
    }
}
