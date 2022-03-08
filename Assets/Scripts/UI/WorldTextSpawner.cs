using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTextSpawner : SingletonBehaviour<WorldTextSpawner>
{
    public static void Spawn(string text, Vector2 position)
    {
        SimpleWorldTextUI worldText = (UIHandler.Temporary.Spawn<SimpleWorldTextUI>(UISpace.World) as SimpleWorldTextUI);
        worldText.Init(text);
        worldText.transform.position = position;
    }
}
