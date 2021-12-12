using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBehaviour : MonoBehaviour, IUpdateMeWithInput
{
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public void HideSelf()
    {
        UIHandler.Instance.Hide(this);
    }

    public virtual void UpdateUI(PlayerInput input) { }
}
