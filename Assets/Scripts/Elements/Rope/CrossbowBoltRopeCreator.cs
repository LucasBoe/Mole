using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowBoltRopeCreator : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    private void Update()
    {
        lineRenderer.SetPosition(0, PlayerRopeUser.Instance.transform.position);
        lineRenderer.SetPosition(1, transform.position);
    }
}
