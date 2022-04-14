using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class StairsUser : MonoBehaviour
{
    List<Stairs> possibleStairs = new List<Stairs>();
    IOrderedEnumerable<Stairs> ordered;
    Stairs inactive = null;


    internal void RegisterPossibleStairs(Stairs stairs)
    {
        possibleStairs.Add(stairs);
        ordered = possibleStairs.OrderBy(stairs => stairs.GetLowestPoint());
    }
    internal void UnregisterPossibleStairs(Stairs stairs)
    {
        possibleStairs.Remove(stairs);

        if (possibleStairs.Count == 0)
            ordered = null;
        else
            ordered = possibleStairs.OrderBy(stairs => stairs.GetLowestPoint());
    }

    private void Update()
    {
        if (inactive == null && (possibleStairs == null || possibleStairs.Count == 0)) return;

        bool isMovingDown = PlayerInputHandler.PlayerInput.Axis.y < 0;

        Stairs newActive = GetNewActiveStairs(isMovingDown);

        if (inactive != newActive)
        {
            if (inactive != null)
                inactive.SetActive(false);

            if (newActive != null)
                newActive.SetActive(true);

            inactive = newActive;
        }
    }

    private void OnDrawGizmos()
    {
        int i = 0;
        foreach (Stairs stairs in possibleStairs)
        {
            Gizmos.color = stairs == inactive ? Color.green : Color.red;
            Gizmos.DrawLine(stairs.transform.position, stairs.otherTarget.position);

            Handles.Label(Vector3.Lerp(stairs.transform.position, stairs.otherTarget.position, 0.5f), i.ToString());
            i++;
        }
    }

    private Stairs GetNewActiveStairs(bool isMovingUp)
    {
        if (ordered == null || ordered.Count() == 0)
            return null;

        for (int i = (isMovingUp ? 0 : ordered.Count() - 1); (i >= 0 || i < ordered.Count()); i = (isMovingUp ? i + 1 : i - 1))
        {
            Stairs stairs = ordered.ToArray()[i];

            Vector2 origin = new Vector2(transform.position.x, transform.position.y - 0.95f);
            Vector2 closest = stairs.GetClosestPoint(origin);

            Util.DebugDrawCircle(origin, Color.cyan, 0.25f);

            Util.DebugDrawCircle(closest, Color.yellow, 0.25f);

            if (closest.y < origin.y)
                return stairs;
        }

        return null;
    }
}
