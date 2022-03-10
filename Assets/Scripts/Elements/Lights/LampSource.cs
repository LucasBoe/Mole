using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LampSource : CollectablePlayerItemWorldObject
{
    private static List<LampSource> lampSources = new List<LampSource>();
    private void OnEnable()
    {
        lampSources.Add(this);
    }

    private void OnDisable()
    {
        lampSources.Remove(this);
    }

    public static LampSource GetClosest(Vector2 position)
    {
        List<LampSource> reachableHeight = new List<LampSource>();

        foreach (LampSource lamp in lampSources)
        {
            if (lamp.IsYClose(position.y))
                reachableHeight.Add(lamp);
            else
                Debug.DrawLine(position, lamp.transform.position, Color.red, 5f);
        }

        if (reachableHeight.Count > 0)
        {
            LampSource[] sorted = reachableHeight.OrderBy(l => Vector2.Distance(l.transform.position, position)).ToArray();
            for (int i = sorted.Length -1; i >= 0; i--)
            {
                Debug.DrawLine(position, sorted[i].transform.position, i == 0 ? Color.green : Color.yellow, 5f);
            }

            return sorted[0];
        }
        return null;
    }

    private bool IsYClose(float y)
    {
        return Mathf.Abs(transform.position.y - y) < 3f;
    }
}
