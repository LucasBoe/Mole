using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBlender : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;

    public void SetActive(bool active)
    {
        canvasGroup.interactable = active;
        canvasGroup.alpha = active ? 1 : 0;
        canvasGroup.blocksRaycasts = active;
    }
}
