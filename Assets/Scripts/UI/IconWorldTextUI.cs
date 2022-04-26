using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconWorldTextUI : SimpleWorldTextUI
{
    [SerializeField] Image iconImage;
    public void Init(Sprite icon, string text)
    {
        iconImage.sprite = icon;
        Init(text);
    }
}
