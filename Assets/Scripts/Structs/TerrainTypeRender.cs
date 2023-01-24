using System;
using UnityEngine;

/// <summary>
/// terrain type name and render 
/// eg, grassland, and render data for grassland
/// and functions
/// </summary>
[Serializable]
public struct TerrainTypeRender
{
    [Tooltip("name of terrain type, eg, Grassland")]
    public string terrainTypeName;
    [Tooltip("rendering data for 13 parts")]
    public RenderedScale13[] clips;  // length might be 13 

    /// <summary>
    /// get rendering data(prefab) using name of scale13 part
    /// </summary>
    public GameObject GetClipPrefab(Scale13 clip)
    {
        foreach (RenderedScale13 tempClip in clips)
        {
            if (tempClip.part == clip)
            {
                return tempClip.prefabForRender;
            }
        }
        return null;
    }
    public static bool operator ==(TerrainTypeRender a, TerrainTypeRender b)
    {
        return a.terrainTypeName == b.terrainTypeName;
    }

    public static bool operator !=(TerrainTypeRender a, TerrainTypeRender b)
    {
        return a.terrainTypeName != b.terrainTypeName;
    }

}
