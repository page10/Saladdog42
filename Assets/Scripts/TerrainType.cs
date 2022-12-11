// public enum TerrainType
//     {
//         Grassland,
//         Water,
//         Forest,
//         RecoverFloor
//     }

using System;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 在逻辑的世界里面，TerrainType并不是一个简单的枚举可以搞定的问题
/// 因为我们需要进一步定义它，“地形类别”在这里更像是一个概括词
/// 理论上来说，我们可以要求地编同学直接用TerrainTile所在的Prefab来拼接地图
/// 但是我们依然可以有另一种拼接地图的方式（比如自己做编辑器，或者随机地图）
/// 所以我们需要这个数据结构，来做到帮助地图（的人类）逻辑信息转化为渲染信息
/// 在人类逻辑里，地图块是“草地”“水”，也就是这个结构的值，但是程序渲染出来，他们却未必一一对应
/// 一个“水”就可能有13种地块（prefab），也就是俗称的十三宫图
/// </summary>
[Serializable]
public struct TerrainType
{
    [Tooltip("这个地形类别的名称，比如草地（Grassland）等")]
    public string name;

    [Tooltip("13宫图的信息")] public TerrainScale13[] clips;

    /// <summary>
    /// 从某个Part获得它的Prefab
    /// </summary>
    /// <param name="part">位置</param>
    /// <returns></returns>
    public GameObject PartPrefab(Scale13 part)
    {
        foreach (TerrainScale13 clip in clips)
        {
            if (clip.part == part)
            {
                return clip.prefab;
            }
        }

        return null;
    }
    
    public static bool operator ==(TerrainType a, TerrainType b)
    {
        return a.name == b.name;
    }

    public static bool operator !=(TerrainType a, TerrainType b)
    {
        return a.name != b.name;
    }
    
}

/// <summary>
/// 地形块的13宫图信息
/// </summary>
[Serializable]
public struct TerrainScale13
{
    /// <summary>
    /// 所属13宫图的位置
    /// </summary>
    public Scale13 part;
    
    /// <summary>
    /// 所使用的Prefab
    /// </summary>
    public GameObject prefab;

    public TerrainScale13(Scale13 part)
    {
        this.part = part;
        this.prefab = null;
    }
}