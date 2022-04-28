using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balista : AboveInputActionProvider
{
    [SerializeField] bool isAiming = false;
    [SerializeField] Transform toRotate;
    [SerializeField] Transform clothlineSpawnPoint;
    [SerializeField] Transform cameraTarget;
    [SerializeField] LineRenderer aimLineRenderer;
    [SerializeField] PlayerItemResource bolt, rope;
    [SerializeField] Rigidbody2D balistaBolt;

    private float targetAngle = 90f;
    private IBalistaTarget currentTarget;

    private void Awake()
    {
        aimLineRenderer.material = new Material(aimLineRenderer.material);
        SetLineAlpha(false);
    }

    protected override InputAction[] CreateInputActions()
    {
        if (isAiming) return null;

        return new InputAction[] { new InputAction() { Text = "Use", Input = ControlType.Use, ActionCallback = Use, Target = transform } };

    }
    private void Use()
    {
        if (!PlayerHasItem(bolt))
        {
            WorldTextSpawner.Spawn(bolt.Sprite, "0/1 " + bolt.name, transform.position);
        }
        else
        {
            StartAiming();
        }
    }

    private bool PlayerHasItem(PlayerItemResource item)
    {
        return PlayerItemHolder.Instance.GetAmount(item) > 0;
    }

    private void StartAiming()
    {
        isAiming = true;
        PlayerInputActionRegister.Instance.UnregisterAllInputActions(transform);
        PlayerInputActionRegister.Instance.RegisterInputAction(new InputAction()
        {
            Text = "Shoot",
            Input = ControlType.Interact,
            Target = transform,
            ActionCallback = Shoot,
        });
        PlayerInputActionRegister.Instance.RegisterInputAction(new InputAction()
        {
            Text = "Abort",
            Input = ControlType.Back,
            Target = transform,
            ActionCallback = StopAiming,
        });

        aimLineRenderer.enabled = true;
        CameraSmartTarget.SetOverride(cameraTarget);
    }

    private void StopAiming()
    {
        isAiming = false;
        PlayerInputActionRegister.Instance.UnregisterAllInputActions(transform);
        PlayerInputActionRegister.Instance.RegisterInputActions(inputActions);


        aimLineRenderer.enabled = false;
        CameraSmartTarget.SetOverride(null);
    }

    private void Shoot()
    {
        if (currentTarget != null)
        {
            if (currentTarget.NeedsRope)
            {
                if (!PlayerHasItem(rope))
                {
                    WorldTextSpawner.Spawn(rope.Sprite, "0/1 " + rope.name, currentTarget.Position);
                }
                else
                {
                    WorldTextSpawner.Spawn("Shoot!", transform.position);
                    ClothlineSpawner.Instance.Spawn(clothlineSpawnPoint.position, currentTarget.Position);
                }
            }
            else
            {
                WorldTextSpawner.Spawn("Shoot!", transform.position);
            }
        }

        Rigidbody2D boltBody = Instantiate(balistaBolt, toRotate.position + toRotate.right, toRotate.rotation, LayerHandler.Parent);
        boltBody.velocity = toRotate.right * 50f;

        PlayerItemHolder.Instance.RemoveItem(bolt);
        StopAiming();
    }

    private void Update()
    {
        Quaternion rotation = Quaternion.Euler(0, 0, targetAngle - 90f);
        toRotate.rotation = rotation;

        Vector2 tip = aimLineRenderer.transform.position;

        aimLineRenderer.SetPosition(0, tip);
        aimLineRenderer.SetPosition(1, tip + 25f * (Vector2)(rotation * Vector3.right));

        if (!isAiming) return;

        Vector2 dir = PlayerInputHandler.PlayerInput.VirtualCursorToDir(tip);
        float z = Mathf.Atan2(dir.x, -dir.y) * Mathf.Rad2Deg;
        targetAngle = Mathf.MoveTowardsAngle(targetAngle, z, Time.deltaTime * 90f);

        Vector2 direction = rotation * Vector3.right;

        cameraTarget.transform.position = (Vector2)transform.position + (direction * 10f);

        List<IBalistaTarget> targets = new List<IBalistaTarget>();

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(tip, direction, 25f, LayerMask.GetMask("Trigger")))
        {
            if (hit.point != null && Vector2.Distance(hit.point, tip) > 1f)
            {
                IBalistaTarget t = hit.collider.GetComponent<IBalistaTarget>();
                if (t != null) targets.Add(t);

                Util.DebugDrawCircle(hit.point, Color.red, 0.5f);
            }
        }

        IBalistaTarget newTarget = targets.Count == 0 ? null : targets[0];

        if (newTarget != currentTarget)
        {
            if (currentTarget != null)
                currentTarget.SetAimAt(false);

            if (newTarget != null)
                newTarget.SetAimAt(true);

            SetLineAlpha(newTarget != null);

            currentTarget = newTarget;
        }
    }

    private void SetLineAlpha(bool full)
    {
        aimLineRenderer.material.color = new Color(1, 1, 1, full ? 1f : 0.5f);
    }
}
