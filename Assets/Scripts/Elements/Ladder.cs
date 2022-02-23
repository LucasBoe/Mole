using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : AboveCooldownInteractable
{
    [SerializeField] BoxCollider2D triggerArea;
    EdgeCollider2D topEdge;
    public bool PlayerIsAbove => playerIsAbove;

    private void Start()
    {
        SetUpEdgCollider();
        LadderClimbState.OnClimbEnter += () => { topEdge.enabled = false; };
        LadderClimbState.OnClimbExit += () => { topEdge.enabled = true; };
    }

    private void SetUpEdgCollider()
    {
        GameObject child = new GameObject("LadderTop");
        child.layer = LayerMask.NameToLayer("OneDirectionalFloor");
        child.transform.parent = transform;
        child.transform.localPosition = new Vector2(0, triggerArea.bounds.size.y / 2 - 0.125f);
        topEdge = child.AddComponent<EdgeCollider2D>();
        topEdge.usedByEffector = true;
        PlatformEffector2D platformEffector2D = child.AddComponent<PlatformEffector2D>();
        platformEffector2D.surfaceArc = 160;
    }

    private void Update()
    {
        if (playerIsAbove && LadderClimbState.CheckEnter())
            PlayerStateMachine.Instance.SetState(new LadderClimbState(this));
    }
}
