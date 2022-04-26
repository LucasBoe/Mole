using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class UIUtil
{
    public static PlayerActionProgressionVisualizerUI SpawnActionProgressionVisualizer()
    {
        return UIHandler.Temporary.Spawn<PlayerActionProgressionVisualizerUI>() as PlayerActionProgressionVisualizerUI;
    }
}
