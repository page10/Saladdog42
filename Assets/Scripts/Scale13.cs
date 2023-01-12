using System;
using UnityEngine;

/// <summary>
/// 13 types of terrain encounters
/// </summary>
public enum Scale13
{
    LeftTop,
    Top,
    RightTop,
    Left,
    Center,
    Right,
    LeftBottom,
    Bottom,
    RightBottom,
    OutLeftTop,
    OutRightTop,
    OutLeftBottom,
    OutRightBottom
}

/// <summary>
/// name of scale 13, and corresponding tile render prefab 
/// </summary>
[Serializable]
public struct RenderedScale13
{
    /// <summary>
    /// name of scale 13 part 
    /// </summary>
    public Scale13 part;

    /// <summary>
    /// corresponding tile render perfab 
    /// </summary>
    public GameObject prefabForRender;
    public RenderedScale13(Scale13 part)
    {
        this.part = part;
        this.prefabForRender = null;
    }

}
