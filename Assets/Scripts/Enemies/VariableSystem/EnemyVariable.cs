using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyVariable
{
    public string Name;
    public Types Type;
    public Vector2 localPosition;
    public Vector2 worldPosition;

    public enum Types
    {
        Position
    }
}
