using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaScanResult
{
    public Vector2 Hit;
    public float Distance;
}

public class AreaScanInformation
{
    public float Angle;
    public AreaScanResult Value;
}

public class AreaScanDetail
{
    public float StartAngle;
    public float EndAngle;
}
