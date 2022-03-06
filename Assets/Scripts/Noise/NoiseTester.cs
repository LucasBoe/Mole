using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTester : MonoBehaviour
{
    private void OnEnable()
    {
        NoiseHandler.Instance.MakeNoise(transform.position, 100);
    }
}
