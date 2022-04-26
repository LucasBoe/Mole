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

    public static void Spawn(Sprite icon, string text, Vector2 position)
    {
        IconWorldTextUI worldText = (UIHandler.Temporary.Spawn<IconWorldTextUI>(UISpace.World) as IconWorldTextUI);
        worldText.Init(icon,text);
        worldText.transform.position = position;
    }
}
