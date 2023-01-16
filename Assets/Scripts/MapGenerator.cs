using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    /// <summary>
    /// map, terrain tiles array
    /// </summary>
    private TileOfTerrain[,] _map;
    public TileOfTerrain[,] Map{
        get{
            return _map;
        }
    }

    /// <summary>
    /// name some prefab tiles 
    /// </summary>
    [Tooltip("preset tile and it name in this map")] public NamedTerrainTile[] namedTerrainTiles;

    public Vector2Int mapSize;

    private void Awake()
    {
        if (namedTerrainTiles == null) namedTerrainTiles = new NamedTerrainTile[0]; // 打个补丁 no null处理 
    }
    private void Start()
    {
        _map = new TileOfTerrain[mapSize.x, mapSize.y];  // initialize map 
        GenerateMap();
    }

    private void GenerateMap()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (x >= 2 && x <= mapSize.x - 2 && y >= 2 && y <= mapSize.y - 2)
                {
                    CreateMapTile(x, y, Random.Range(0, 1.000f) < 0.8f ? "Water" : "Forest");
                }
                else
                {
                    CreateMapTile(x, y, "Grassland");  // 草草 这里写得不是很合适 
                }
            }
        }
        ReRenderTiles();
    }

    private void CreateMapTile(int x, int y, string tileName)
    {
        foreach (NamedTerrainTile namedTile in namedTerrainTiles)
        {
            // Debug.Log(tileName);
            // Debug.Log("namedTile.name" + namedTile.name);
            if (namedTile.name == tileName)
            {
                CreateMapTile(x, y, namedTile.prefabTile);
                //Debug.Log(tileName);
            }
        }

    }
    private void CreateMapTile(int x, int y, TileOfTerrain namedTile)
    {
        //单元格位置不对，不予创建
        if (x < 0 || x < 0 || x >= mapSize.x || x >= mapSize.y || !namedTile) return;

        if (_map[x, y] != null)
        {
            Destroy(_map[x, y].gameObject);
        }
        GameObject gameObject = Instantiate(namedTile.gameObject);
        if (gameObject)
        {
            _map[x, y] = gameObject.GetComponent<TileOfTerrain>();
            //Debug.Log(_map[x, y].terrainType.terrainTypeName);
            gameObject.transform.SetParent(transform);
            GridPosition gridPosition = gameObject.GetComponent<GridPosition>();
            if (gridPosition)
            {
                gridPosition.grid = new Vector2Int(x, y);
                gridPosition.SynchronizeGridPosition();
            }
        }
    }

    private void ReRenderTiles()
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
                //Unity和全世界游戏的坐标系Y方向是反的
                bool leftSame = i > 0 && _map[i - 1, j].terrainType == _map[i, j].terrainType;
                bool leftTopSame = i > 0 && j < mHeight - 1 && _map[i - 1, j + 1].terrainType == _map[i, j].terrainType;
                bool topSame = j < mHeight - 1 && _map[i, j + 1].terrainType == _map[i, j].terrainType;
                bool rightTopSame = i < mWidth - 1 && j < mHeight - 1 && _map[i + 1, j + 1].terrainType == _map[i, j].terrainType;
                bool rightSame = i < mWidth - 1 && _map[i + 1, j].terrainType == _map[i, j].terrainType;
                bool rightBottomSame = i < mWidth - 1 && j > 0 && _map[i + 1, j - 1].terrainType == _map[i, j].terrainType;
                bool bottomSame = j > 0 && _map[i, j - 1].terrainType == _map[i, j].terrainType;
                bool leftBottomSame = i > 0 && j > 0 && _map[i - 1, j - 1].terrainType == _map[i, j].terrainType;
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
}


