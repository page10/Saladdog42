using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 操作可以覆盖的格子 
/// </summary>
public struct CoveredRange
{
    public byte targetType;
    public Vector2Int gridPos;
    public static CoveredRange MixedType(CoveredRange o, byte target)
    {
        byte b = (byte)(o.targetType | target);
        return new CoveredRange(b, o.gridPos);
    }

    public CoveredRange(byte targetType, Vector2Int gridPos)
    {
        this.targetType = targetType;
        this.gridPos = gridPos;
    }
}
