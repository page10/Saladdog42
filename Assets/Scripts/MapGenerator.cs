using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    // 先只弄水 等加森林的时候 在这里加一个list 加的时候加一个判定看用哪个list
    private Texture2D[] _waterTextures;
    public RectTransform tile_Template;
    public RectTransform tile_Grass;
    public TerrainTile[,] tileMap = new TerrainTile[Constants.maxTilesX, Constants.maxTilesY];
    TerrainType[,] typeMap = new TerrainType[Constants.maxTilesX, Constants.maxTilesY];  // 这个是不是可以不要？
    Dictionary<Vector2Int, RectTransform> presentWaterTiles = new Dictionary<Vector2Int, RectTransform>();
    
    public float TileSize
    {
        get { return Constants.tileSize; }
    }
    enum TileParts
    {
        LU,
        RU,
        LD,
        RD,
        count
    };

    /// <summary>
    /// 地图数组
    /// </summary>
    private TerrainTile[,] _map;

    /// <summary>
    /// 我们预设一些地图块，看实现代码里的“写死效果” 
    /// </summary>
    [Tooltip("预设的地图块")] public NamedTerrainTile[] prefabTiles;

    /// <summary>
    /// 可以把地图尺寸暴露给用户去设置
    /// </summary>
    [Tooltip("地图的尺寸")]public Vector2Int mapSize;

    private void Awake()
    {
        if (prefabTiles == null) prefabTiles = new NamedTerrainTile[0];
    }

    void Start()
    {
        //上来我们先建立好地图的数据
        _map = new TerrainTile[mapSize.x, mapSize.y];
        
        //生成地图
        GenerateMap();
        // typeMap[2, 4] = typeMap[3, 3] = TerrainType.Grassland;  // 插一块草感受一下
        //
        // for(int x = 0; x < Constants.maxTilesX; x++)
        // {
        //     for (int y = 0; y < Constants.maxTilesY; y++)
        //     {
        //         if (typeMap[x, y] == TerrainType.Water)
        //         {
        //             BuildWaterTerrain(x, y);
        //         }
        //         else if (typeMap[x, y] == TerrainType.Grassland)
        //         {
        //             BuildGrassTerrain(x, y);
        //         }
        //         
        //     }
        // }
    }
    private void GenerateMap()  // Generate different types of tiles 
    {
        for (int x = 0; x < mapSize.x ; x++)
        {
            for (int y = 0; y < mapSize.y ; y++)
            {
                //保持原来的设计意图
                if (x >= 2 && x < mapSize.x - 2 && y >= 2 && y < mapSize.y - 2)
                {
                    //typeMap[x, y] =  TerrainType.Water;
                    //改成80%几率是水试试看？
                    CreateMapTile(x, y, Random.Range(0, 1.000f) < 0.8f ? "Water" : "Grassland");
                }
                else
                {
                    CreateMapTile(x, y, "Grassland");
                }
                
            }
        }
        
        FixMapRenderTiles();
    }
    
    //------------------------------------------------------------------------------------------------------
    // 关于地图的创建，我们创建地图块需要抽象，任何地图块其实都是一样的
    // 因此这一层并不存在“草地”“水”这样的概念，我们要做的只是创建地图块
    // 也就是某个单元格里面，创建了某个Prefab的东西，创建完成之后，我们需要对地图修一下
    //-------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 在指定单元格创建一个地图块
    /// </summary>
    /// <param name="gridX">单元格x</param>
    /// <param name="gridY">单元格y</param>
    /// <param name="tile">格子</param>
    private void CreateMapTile(int gridX, int gridY, TerrainTile tile)
    {
        //单元格位置不对，不予创建
        if (gridX < 0 || gridY < 0 || gridX >= mapSize.x || gridY >= mapSize.y || !tile) return;

        //丢掉老的
        if (_map[gridX, gridY] != null)
        {
            Destroy(_map[gridX, gridY].gameObject);
        }

        //创建新的
        GameObject go = Instantiate(tile.gameObject);
        if (go)
        {
            _map[gridX, gridY] = go.GetComponent<TerrainTile>();
            go.transform.SetParent(transform);
            go.transform.position = new Vector3(
                gridX * Constants.tileSize,
                gridY * Constants.tileSize
            );
        }
    }

    /// <summary>
    /// 更便捷的创建地图块，直接用prefabTiles里面的内容
    /// 所以，限制也是必须prefabTiles里面得有这个
    /// </summary>
    /// <param name="gridX">单元格x</param>
    /// <param name="gridY">单元格y</param>
    /// <param name="tileName">格子名称</param>
    private void CreateMapTile(int gridX, int gridY, string tileName)
    {
        TerrainTile tile = null;
        
        foreach (NamedTerrainTile prefabTile in prefabTiles)
        {
            if (prefabTile.name == tileName)
            {
                tile = prefabTile.terrainTile;
                break;
            }
        }

        CreateMapTile(gridX, gridY, tile);
    }

    /// <summary>
    /// "修复"一下地图上所有的贴图，也就是根据地图内容，来决定每个角落贴图是什么
    /// </summary>
    private void FixMapRenderTiles()
    {
        //地图宽高（实际的，而非输入数据）
        int mWidth = _map.GetLength(0);
        int mHeight = _map.GetLength(1);
        
        //每个地图块的Scale13[2,2]信息, left<<2 & top<<1 & leftTop, 这和42=101010是一样的原理, 0代表不同，1代表相同
        Dictionary<int, Scale13> leftTopScale13s = new Dictionary<int, Scale13>();
        leftTopScale13s.Add(0, Scale13.LeftTop);   
        leftTopScale13s.Add(1, Scale13.LeftTop); 
        leftTopScale13s.Add(2, Scale13.Left);   
        leftTopScale13s.Add(3, Scale13.Left);  
        leftTopScale13s.Add(4, Scale13.Top);
        leftTopScale13s.Add(5, Scale13.Top);
        leftTopScale13s.Add(6, Scale13.OutLeftTop);    //事实上，这一格可以是跟左上相同地形接壤的，但这需要Scale17（就跟RPGMaker一样了，可以试试看扩展）
        leftTopScale13s.Add(7, Scale13.Center);

        //right << 2 & top << 1 & rightTop
        Dictionary<int, Scale13> rightTopScale13s = new Dictionary<int, Scale13>();
        rightTopScale13s.Add(0, Scale13.RightTop);   
        rightTopScale13s.Add(1, Scale13.RightTop); 
        rightTopScale13s.Add(2, Scale13.Right);   
        rightTopScale13s.Add(3, Scale13.Right);  
        rightTopScale13s.Add(4, Scale13.Top);
        rightTopScale13s.Add(5, Scale13.Top);
        rightTopScale13s.Add(6, Scale13.OutRightTop); 
        rightTopScale13s.Add(7, Scale13.Center);
        
        //left << 2 & bottom << 1 & rightTop
        Dictionary<int, Scale13> leftBottomScale13s = new Dictionary<int, Scale13>();
        leftBottomScale13s.Add(0, Scale13.LeftBottom);   
        leftBottomScale13s.Add(1, Scale13.LeftBottom); 
        leftBottomScale13s.Add(2, Scale13.Left);   
        leftBottomScale13s.Add(3, Scale13.Left);  
        leftBottomScale13s.Add(4, Scale13.Bottom);
        leftBottomScale13s.Add(5, Scale13.Bottom);
        leftBottomScale13s.Add(6, Scale13.OutLeftBottom);
        leftBottomScale13s.Add(7, Scale13.Center);
        
        //right << 2 & bottom << 1 & rightTop
        Dictionary<int, Scale13> rightBottomScale13s = new Dictionary<int, Scale13>();
        rightBottomScale13s.Add(0, Scale13.RightBottom);   
        rightBottomScale13s.Add(1, Scale13.RightBottom); 
        rightBottomScale13s.Add(2, Scale13.Right);   
        rightBottomScale13s.Add(3, Scale13.Right);  
        rightBottomScale13s.Add(4, Scale13.Bottom);
        rightBottomScale13s.Add(5, Scale13.Bottom);
        rightBottomScale13s.Add(6, Scale13.OutRightBottom);
        rightBottomScale13s.Add(7, Scale13.Center);

        //获得每个格子贴图
        for (int i = 0; i < mWidth ; i++)
        {
            for (int j = 0; j < mHeight; j++)
            {
                if (!_map[i, j]) continue;
                
                //Unity和全世界游戏的坐标系Y方向是反的
                bool leftSame = i > 0 && _map[i - 1, j].terrain == _map[i, j].terrain;
                bool leftTopSame = i > 0 && j < mHeight - 1 && _map[i - 1, j + 1].terrain == _map[i, j].terrain;
                bool topSame = j < mHeight - 1 && _map[i, j + 1].terrain == _map[i, j].terrain;
                bool rightTopSame = i < mWidth - 1 && j < mHeight - 1 && _map[i + 1, j + 1].terrain == _map[i, j].terrain;
                bool rightSame = i < mWidth - 1 && _map[i + 1, j].terrain == _map[i, j].terrain;
                bool rightBottomSame = i < mWidth - 1 && j > 0 && _map[i + 1, j - 1].terrain == _map[i, j].terrain;
                bool bottomSame = j > 0 && _map[i, j - 1].terrain == _map[i, j].terrain;
                bool leftBottomSame = i > 0 && j > 0 && _map[i - 1, j - 1].terrain == _map[i, j].terrain;
                Scale13[,] subTile = new Scale13[2, 2];
                subTile[0, 0] =
                    leftTopScale13s[(leftSame == true ? (1 << 2) : 0) | (topSame == true  ? (1 << 1) : 0) | (leftTopSame == true  ? 1 : 0)];
                subTile[1, 0] =
                    rightTopScale13s[(rightSame == true ? (1 << 2) : 0) | (topSame == true  ? (1 << 1) : 0) | (rightTopSame == true  ? 1 : 0)];
                subTile[0, 1] =
                    leftBottomScale13s[(leftSame == true  ? (1 << 2) : 0) | (bottomSame == true  ? (1 << 1) : 0) | (leftBottomSame == true  ? 1 : 0)];
                subTile[1, 1] =
                    rightBottomScale13s[(rightSame == true  ? (1 << 2) : 0) | (bottomSame == true  ? (1 << 1) : 0) | (rightBottomSame == true  ? 1 : 0)];
                _map[i, j].SetSubTile(subTile);
                
                
            }
        }
    }
    
    private void BuildGrassTerrain(int x, int y) 
    {
        var newTile = Instantiate(tile_Grass.gameObject).GetComponent<RectTransform>();           
        newTile.SetParent(tile_Grass.parent);
        newTile.anchoredPosition = new Vector2(x * TileSize, y * TileSize);
    }

    private void BuildWaterTerrain(int x, int y) 
    {
        var newTile = Instantiate(tile_Template.gameObject).GetComponent<RectTransform>();           
        newTile.SetParent(tile_Template.parent);
        newTile.anchoredPosition = new Vector2(x * TileSize, y * TileSize);
        Vector2Int tileIndex = new Vector2Int(x, y);
        presentWaterTiles.Add(tileIndex, newTile);
        RefreshWaterTexture();
    }
    
    private void RefreshWaterTexture()
    {
       foreach (var tile in presentWaterTiles)
       {
            for (int i = 0; i < (int)TileParts.count; i++)
            {
                TileParts part = (TileParts)i;
                tile.Value.Find(part.ToString()).GetComponent<UnityEngine.UI.RawImage>().texture
                    = _waterTextures[GetExpectElementIndex(tile.Key,part)];
            }
       }
    }

    int GetExpectElementIndex(Vector2Int TileIndex, TileParts part)
    {
        var NeighbourFilled = new System.Func<Vector2Int,bool >((NeighbourOffset) =>
        {
            return presentWaterTiles.ContainsKey(TileIndex + NeighbourOffset);
        });

        if(part==TileParts.LU)
        {
            bool up = NeighbourFilled(new Vector2Int(0, 1));
            bool left = NeighbourFilled(new Vector2Int(-1, 0));
            bool up_left = NeighbourFilled(new Vector2Int(-1, 1));
            if (up && left)//上和左都有
            {
                if (up_left) //左上也有
                    return 19;
                else                                        //左上没有
                    return 5;
            }
            else if (!(up || left))//上和左都没有
                return 9;
            else    //左和上有一者有
            {
                if (up)         //上
                    return 17;
                else            //左
                    return 11;
            }
        }
        if (part == TileParts.RU)
        {
            bool up = NeighbourFilled(new Vector2Int(0, 1));
            bool right = NeighbourFilled(new Vector2Int(1, 0));
            bool up_right = NeighbourFilled(new Vector2Int(1, 1));
            if (up && right)//上和右都有
            {
                if (up_right) //右上也有
                    return 18;
                else                                        //右上没有
                    return 6;
            }
            else if (!(up || right))//上和右都没有
                return 12;
            else    //右和上有一者有
            {
                if (up)         //上
                    return 20;
                else            //右
                    return 10;
            }
        }
        if (part == TileParts.LD)
        {
            bool down = NeighbourFilled(new Vector2Int(0, -1));
            bool left = NeighbourFilled(new Vector2Int(-1, 0));
            bool down_left = NeighbourFilled(new Vector2Int(-1, -1));
            if (down && left)//下和左都有
            {
                if (down_left) //左下也有
                    return 15;
                else                                        //左下没有
                    return 7;
            }
            else if (!(down || left))//下和左都没有
                return 21;
            else    //左和下有一者有
            {
                if (down)         //下
                    return 13;
                else            //左
                    return 23;
            }
        }
        if (part == TileParts.RD)
        {
            bool down = NeighbourFilled(new Vector2Int(0, -1));
            bool right = NeighbourFilled(new Vector2Int(1, 0));
            bool down_right = NeighbourFilled(new Vector2Int(1, -1));
            if (down && right)//下和右都有
            {
                if (down_right) //右下也有
                    return 14;
                else                                        //右下没有
                    return 8;
            }
            else if (!(down || right))//下和右都没有
                return 24;
            else    //右和下有一者有
            {
                if (down)         //下
                    return 16;
                else            //右
                    return 22;
            }
        }

        return 0;
    }
}
