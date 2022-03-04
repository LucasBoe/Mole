using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundCheckModule : EnemyModule<EnemyGroundCheckModule>
{
    [SerializeField, Layer]
    List<int> validLayers = new List<int>();
    [SerializeField, Layer, ReadOnly]
    List<int> currentLayers = new List<int>();

    public System.Action EnteredGround;
    public System.Action LeftGround;
    public bool IsGrounded => currentLayers.Count != 0;
    [SerializeField, ReadOnly] private float groundTime = 10f;
    public float GroundTime => groundTime;

    [SerializeField, ReadOnly] private bool enableCorrectio = false;

    private EnemyRigidbodyControllerModule rigidbodyControllerModule;

    private void Start()
    {
        rigidbodyControllerModule = GetModule<EnemyRigidbodyControllerModule>();
        if (rigidbodyControllerModule != null)
            rigidbodyControllerModule.FallmodeChanged += OnFallmodeChanged;

    }

    private void OnDestroy()
    {
        if (rigidbodyControllerModule != null)
            rigidbodyControllerModule.FallmodeChanged -= OnFallmodeChanged;
    }

    private void OnFallmodeChanged(bool fallModeActive)
    {
        enableCorrectio = fallModeActive;
    }

    private void LateUpdate()
    {
        if (enableCorrectio)
            transform.rotation = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        if (IsGrounded)
            groundTime += Time.fixedDeltaTime;
        else
            groundTime = 0;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layer = collision.gameObject.layer;
        if (!currentLayers.Contains(layer) && validLayers.Contains(layer))
        {
            currentLayers.Add(layer);
            if (currentLayers.Count == 1)
            {
                groundTime = 0f;
                EnteredGround?.Invoke();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        int layer = collision.gameObject.layer;
        if (currentLayers.Contains(layer))
        {
            currentLayers.Remove(layer);
            if (!IsGrounded)
                this.Delay(0.01f, CheckForGround);
        }
    }

    private void CheckForGround()
    {
        if (!IsGrounded)
        {
            LeftGround?.Invoke();
        }
    }
}
