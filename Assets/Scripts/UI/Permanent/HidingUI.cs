using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HidingUI : MonoBehaviour
{
    [SerializeField] Image image;

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = PlayerHidingHandler.Instance.PlayerHiddenValue;
    }
}
