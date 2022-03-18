using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : AboveCooldownInteractable
{
    [SerializeField] BoxCollider2D triggerArea;
    EdgeCollider2D topEdge;
    public bool PlayerIsAbove => playerIsAbove;
    public Vector2 ExitPointTop, ExitPointBottom;

    private void Start()
    {
        SetUpEdgCollider();
        ExitPointTop = GetExitPointTop();
        ExitPointBottom = GetExitPointBottom();
    }

    private void SetUpEdgCollider()
    {
        GameObject child = new GameObject("LadderTop");
        child.layer = LayerMask.NameToLayer("HangableCollidable");
        child.transform.parent = transform;
        child.transform.localPosition = new Vector2(0, triggerArea.bounds.size.y / 2 - 0.125f);
        topEdge = child.AddComponent<EdgeCollider2D>();
        topEdge.usedByEffector = true;
        PlatformEffector2D platformEffector2D = child.AddComponent<PlatformEffector2D>();
        platformEffector2D.surfaceArc = 160;
    }

    private void Update()
    {
        if (playerIsAbove && LadderClimbState.CheckEnter(this))
            PlayerStateMachine.Instance.SetState(new LadderClimbState(this));
    }

    private Vector2 GetExitPointTop()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y + triggerArea.size.y / 2f + 1f);
        while (Physics2D.OverlapBox(pos, new Vector2(0.9f, 2f), 0, LayerMask.GetMask("Default")) != null)
            pos += Vector2.down;

        return pos;
    }

    private Vector2 GetExitPointBottom()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y - triggerArea.size.y / 2f - 1f);
        while (Physics2D.OverlapBox(pos + new Vector2(0, 0.25f), new Vector2(0.9f, 2f), 0, LayerMask.GetMask("Default")) != null)
            pos += Vector2.up;

        return pos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(GetExitPointBottom(), 0.5f);
        Gizmos.DrawSphere(GetExitPointTop(), 0.5f);
    }
}
