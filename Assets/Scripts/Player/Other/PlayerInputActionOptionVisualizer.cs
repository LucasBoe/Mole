using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInputActionOptionVisualizer : MonoBehaviour
{
    [SerializeField] RectTransform worldSpaceParent, uiSpaceParent;
    [SerializeField] Vector3 uiSpaceOffset, worldSpaceOffset;
    [SerializeField] RectTransform inputUIParentPrefab;
    [SerializeField] PlayerControlPromptUI inputUIPrefab;

    Dictionary<UnityEngine.Object, RectTransform> uiParents = new Dictionary<UnityEngine.Object, RectTransform>();
    Dictionary<ControlType, KeyValuePair<PlayerControlPromptUI, InputAction>> uiInstances = new Dictionary<ControlType, KeyValuePair<PlayerControlPromptUI, InputAction>>();

    private void Start()
    {
        PlayerInputActionRegister.OnInputActionChangedForType += UpdateUI;
        PlayerControlPromptUI.OnDeleteControPrompt += CheckForDeletionIfNoChildsExist;
    }

    private void CheckForDeletionIfNoChildsExist(Transform parent)
    {
        if (parent.childCount == 1)
        {
            UnityEngine.Object key = null;

            if (uiParents.ContainsValue(parent as RectTransform))
            {
                foreach (var pair in uiParents)
                {
                    if (pair.Value == parent)
                    {
                        key = pair.Key;
                        break;
                    }
                }
            }

            uiParents.Remove(key);

            Destroy(parent.gameObject);
        }
    }

    private void UpdateUI(ControlType type)
    {
        InputAction top = PlayerInputActionRegister.GetTopActionForType(type);
        PlayerControlPromptUI prompt = null;

        if (uiInstances.ContainsKey(type)) prompt = uiInstances[type].Key;

        if (top == null)
        {
            if (prompt != null)
            {
                Transform parent = prompt.transform.parent;
                prompt.Hide();
            }
            uiInstances.Remove(type);
        }
        else
        {
            if (prompt == null)
            {
                prompt = Instantiate(inputUIPrefab);
            }
            uiInstances[type] = new KeyValuePair<PlayerControlPromptUI, InputAction>(prompt, top);

            prompt.Init(type, Vector3.zero, top.Text);
            Position(top.TargetTransform, prompt, top.TargetType);
        }
    }



    private void Position(Transform targetTransform, PlayerControlPromptUI prompt, InputAction.TargetTypes targetType)
    {
        RectTransform parent;

        if (uiParents.ContainsKey(targetTransform))
            parent = uiParents[targetTransform];
        else
        {
            parent = Instantiate(inputUIParentPrefab, targetType == InputAction.TargetTypes.RectTransform ? uiSpaceParent : worldSpaceParent);
            uiParents.Add(targetTransform, parent);
        }

        parent.position = targetTransform.position + (targetType == InputAction.TargetTypes.RectTransform ? uiSpaceOffset : worldSpaceOffset);
        prompt.transform.parent = parent;
        prompt.transform.localScale = Vector3.one;
    }
}
