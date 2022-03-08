using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleWorldTextUI : TemporaryUIElement
{
    [SerializeField] TMP_Text textText;

    public void Init(string text)
    {
        textText.text = text;
    }
}
