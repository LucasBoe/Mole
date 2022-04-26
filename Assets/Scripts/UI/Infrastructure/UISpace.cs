using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpace : SingletonBehaviour<UISpace>
{
    [SerializeField] Transform world, ui;
    public static Transform World => Instance.world;
    public static Transform UI => Instance.ui;
}
