using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public class Dumpster : AboveInputActionProvider
{
    [Foldout("Settings"), SerializeField] SpriteRenderer spriteRenderer;
    [Foldout("Settings"), SortingLayer, SerializeField] int behindPlayer, inFrontOfPlayer;
    [Foldout("Settings"), SerializeField] EdgeCollider2D hiddenEdgeCollider;

    [SerializeField] PlayerItem content;
    private bool isSearching = false;

    InputAction searchAction;
    protected override void Start()
    {
        searchAction = new InputAction() { ActionCallback = StartSearch, Input = ControlType.Interact, Stage = InputActionStage.WorldObject, Target = transform, Text = "Search" };
        base.Start();
        HideEdge(false);
    }
    protected override InputAction[] CreateInputActions()
    {
        return new InputAction[] { searchAction };
    }
    private void StartSearch()
    {
        OnPlayerExit();
        StopAllCoroutines();
        StartCoroutine(SearchRoutine());
    }

    protected override void OnPlayerEnter()
    {
        if (content != null && !isSearching) base.OnPlayerEnter();
    }

    private IEnumerator SearchRoutine()
    {
        float searchDuration = 2f, t = 0f;
        PlayerActionProgressionVisualizerUI uiElement = UIUtil.SpawnActionProgressionVisualizer();
        isSearching = true;
        while (t < searchDuration)
        {
            t += Time.deltaTime;
            uiElement.UpdaValue(t / searchDuration);
            yield return null;
        }
        isSearching = false;
        uiElement.Hide();

        PlayerItemHolder.Instance.AddItem(content);
        content = null;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        HidingState.EnterState += OnEnterState;
        HidingState.ExitState += OnExitState;
    }

    private void OnDisable()
    {
        HidingState.EnterState -= OnEnterState;
        HidingState.ExitState -= OnExitState;
    }
    private void OnEnterState()
    {
        spriteRenderer.sortingLayerID = inFrontOfPlayer;
    }
    private void OnExitState()
    {
        spriteRenderer.sortingLayerID = behindPlayer;
    }

    private void OnValidate()
    {
        HideEdge(true);
    }

    private void HideEdge(bool hideEdge)
    {
        foreach (Transform item in transform)
        {
            item.gameObject.hideFlags = HideFlags.None;
        }
    }
}
