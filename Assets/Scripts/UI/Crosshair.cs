using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : SingletonBehaviour<Crosshair>
{
    Image crosshairUIImage;
    static Dictionary<Mode, float> modeOpacity;
    private void OnEnable()
    {
        crosshairUIImage = GetComponent<Image>();
        modeOpacity = new Dictionary<Mode, float>();
        modeOpacity.Add(Mode.Hidden, 0f);
        modeOpacity.Add(Mode.Passive, 0.33f);
        modeOpacity.Add(Mode.Active, 1f);
    }

    public enum Mode
    {
        Active,
        Passive,
        Hidden,
    }

    void Update()
    {
        transform.position = PlayerInputHandler.PlayerInput.VirtualCursor;
    }

    public static void SetMode(Mode mode)
    {
        Instance.crosshairUIImage.color = new Color(1, 1, 1, modeOpacity[mode]);
    }
}
