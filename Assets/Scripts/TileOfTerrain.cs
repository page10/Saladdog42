using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a tile, represents a kind of terrain
/// describe a logic tile 
/// specifically, movement cost and modifier of character 
/// and render data 
/// contains 2 by 2 subtiles
/// </summary>
public class TileOfTerrain : MonoBehaviour
{
    /// <summary>
    /// move cost list, for each type of character, on this terrain
    /// </summary>
    [Tooltip("move cost for each type of character, not set represents impassable")]
    public  MovementInfo[] moveCosts;

    /// <summary>
    /// character properties modifier, per centum
    /// add another structure like this if need to modify directly
    /// </summary>
    [Tooltip("character properties modifier, per centum, 0 for default")]
    public CharacterStatus percentageModifier;

    /// <summary>
    /// map render data of this type of tile
    /// </summary>
    public TerrainTypeRender terrainType;

    /// <summary>
    /// 4 parts of a tile, render reference 
    /// </summary>
    private RenderTile[,] _subTiles = new RenderTile[2,2];

    /// <summary>
    /// set render data for 4 parts of a tile
    /// </summary>
    /// <param name="part"> name of the part </param>
    public void SetSubTile(Scale13[,] part)
    {        
        //异常处理，传进来的不是一个2x2的，就return了
        if (part.GetLength(0) != 2 || part.GetLength(1) != 2) return;
        
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                //destroy previous render data
                if (_subTiles[i, j] != null) Destroy(_subTiles[i, j].gameObject);
                
                //create new render data
                GameObject go = Instantiate(terrainType.GetClipPrefab(part[i, j]));
                if (go)
                {
                    _subTiles[i, j] = go.GetComponent<RenderTile>();
                    go.transform.SetParent(transform);
                    go.transform.localPosition = new Vector3(
                        i * Constants.tileSize * 0.500f,
                        (1 - j) * Constants.tileSize * 0.500f       //unity和全世界相反
                    );
                }
            }
        }
    }

}

/// <summary>
/// name different tile prefabs for different maps 
/// key - value struct 
/// </summary>
[Serializable]
public struct NamedTerrainTile
{
    [Tooltip("地图块的名称")] public string name;
    [Tooltip("地图块")] public TileOfTerrain prefabTile;

}